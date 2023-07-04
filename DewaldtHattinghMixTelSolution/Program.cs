using System.Diagnostics;

namespace DewaldtHattinghMixTelSolution
{
    internal abstract class Program
    {
        private static void Main()
        {
            Stopwatch timer = new Stopwatch();
            
            timer.Start();
            var geoCoordinates = ObtainLookupCoordinates();
            var vehiclePositions = DataInterpreter.ParseData();
             
            foreach (var geoCoordinate in geoCoordinates)
            {
                var closestPosition = IdentifyClosestPosition(geoCoordinate, vehiclePositions);
                Console.WriteLine(
                    $"Closest position to {geoCoordinate.Latitude}, {geoCoordinate.Longitude} is {closestPosition}");
            }
            timer.Stop();
            Console.WriteLine("Elapsed Time: " + timer.ElapsedMilliseconds +"ms");
            Console.WriteLine("Press any key to close the app!");
            Console.ReadKey();
        }

        private static int IdentifyClosestPosition(Coordinate geoCoordinate, List<VehiclePositionData> vehiclePositions)
        {
            var minimumDistance = double.MaxValue;
            int closestPosition = 0;
            var lowerBound = 0;
            var upperBound = vehiclePositions.Count - 1;

            while (lowerBound <= upperBound)
            {
                var median = (lowerBound + upperBound) / 2;

                var distance = ComputeDistance(geoCoordinate.Latitude, geoCoordinate.Longitude, vehiclePositions[median].Latitude,
                    vehiclePositions[median].Longitude);

                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    closestPosition = vehiclePositions[median].PositionId;
                }

                if (distance < 0)
                {
                    upperBound = median - 1;
                }
                else
                {
                    lowerBound = median + 1;
                }
            }

            return closestPosition;
        }

        private static List<Coordinate> ObtainLookupCoordinates()
        {
            return new List<Coordinate>
            {
                new Coordinate(34.544909, -102.100843),
                new Coordinate(32.345544, -99.123124),
                new Coordinate(33.234235, -100.214124),
                new Coordinate(35.195739, -95.348899),
                new Coordinate(31.895839, -97.789573),
                new Coordinate(32.895839, -101.789573),
                new Coordinate(34.115839, -100.225732),
                new Coordinate(32.335839, -99.992232),
                new Coordinate(33.535339, -94.792232),
                new Coordinate(32.234235, -100.222222)
            };
        }

        private static double ComputeDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double angle1 = lat1 * (Math.PI / 180.0);
            double angle2 = lon1 * (Math.PI / 180.0);
            double angle3 = lat2 * (Math.PI / 180.0);
            double angleDiff = lon2 * (Math.PI / 180.0) - angle2;
            double distance = Math.Pow(Math.Sin((angle3 - angle1) / 2.0), 2.0) +
                        Math.Cos(angle1) * Math.Cos(angle3) * Math.Pow(Math.Sin(angleDiff / 2.0), 2.0);
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(distance), Math.Sqrt(1.0 - distance)));
        }
    }
}