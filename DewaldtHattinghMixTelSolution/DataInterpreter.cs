using System.Reflection;
using System.Text;

namespace DewaldtHattinghMixTelSolution;

public static class DataInterpreter
{
    public static List<VehiclePositionData> ParseData()
    {
        string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        string fullPath      = Path.Combine(directoryPath, "VehiclePositions.dat");

        List<VehiclePositionData> vehiclePositions = new List<VehiclePositionData>(2000000);

        using FileStream   fileStream   = new FileStream(fullPath, FileMode.Open);
        using BinaryReader binaryReader = new BinaryReader(fileStream);
        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
        {
            VehiclePositionData positionData = new VehiclePositionData
                                               {
                                                   PositionId          = binaryReader.ReadInt32(),
                                                   VehicleRegistration = GetStringFromBinary(binaryReader),
                                                   Latitude            = binaryReader.ReadSingle(),
                                                   Longitude           = binaryReader.ReadSingle(),
                                                   RecordedTimeUTC     = binaryReader.ReadUInt64()
                                               };

            vehiclePositions.Add(positionData);
        }

        return vehiclePositions;
    }

    private static string GetStringFromBinary(BinaryReader readerInstance)
    {
        var  byteDataList = new List<byte>();
        byte currentByte;
        while ((currentByte = readerInstance.ReadByte()) != 0)
        {
            byteDataList.Add(currentByte);
        }

        return Encoding.UTF8.GetString(byteDataList.ToArray());
    }
}