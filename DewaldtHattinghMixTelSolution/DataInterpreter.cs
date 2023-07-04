using System.Reflection;
using System.Text;

namespace DewaldtHattinghMixTelSolution;

 public static class DataInterpreter
    {
        private static string GetStringFromBinary(BinaryReader readerInstance)
        {
            var byteDataList = new List<byte>();
            byte currentByte;
            while ((currentByte = readerInstance.ReadByte()) != 0)
            {
                byteDataList.Add(currentByte);
            }

            return Encoding.UTF8.GetString(byteDataList.ToArray());
        }

        public static List<VehiclePositionData> ParseData()
        {
            var vehiclePositionList = new List<VehiclePositionData>();
            try
            {
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
                string fullPath = Path.Combine(directoryPath, "VehiclePositions.dat");

                using var fileStreamInstance = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                using var readerInstance = new BinaryReader(new BufferedStream(fileStreamInstance));

                int definedChunkSize = 4096;
                var dataBuffer = new byte[definedChunkSize];
                int numBytesRead;
                while ((numBytesRead = readerInstance.Read(dataBuffer, 0, dataBuffer.Length)) > 0)
                {
                    using var memoryStreamInstance = new MemoryStream(dataBuffer, 0, numBytesRead);
                    using var binaryReaderInstance = new BinaryReader(memoryStreamInstance);
                    try
                    {
                        while (memoryStreamInstance.Position < memoryStreamInstance.Length)
                        {
                            var positionId = binaryReaderInstance.ReadInt32();
                            var vehicleReg = GetStringFromBinary(binaryReaderInstance);
                            var latVal = binaryReaderInstance.ReadSingle();
                            var lonVal = binaryReaderInstance.ReadSingle();
                            var utcTimeRecorded = binaryReaderInstance.ReadUInt64();

                            var vehiclePositionData = new VehiclePositionData()
                            {
                                PositionId = positionId,
                                VehicleRegistration = vehicleReg,
                                Latitude = latVal,
                                Longitude = lonVal,
                                RecordedTimeUTC = utcTimeRecorded
                            };
                            vehiclePositionList.Add(vehiclePositionData);
                        }
                    }
                    catch (EndOfStreamException exception)
                    {
                        Console.WriteLine("DataInterpreter.cs: " + exception.Message);
                        break;
                    }
                }
            }
            catch (Exception generalException)
            {
                Console.WriteLine(generalException.Message);
            }

            return vehiclePositionList;
        }
    }