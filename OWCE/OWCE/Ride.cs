using System;
using System.IO;
using SQLite;
using Xamarin.Essentials;

namespace OWCE
{
    public class Ride
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public string Name { get; set; }

        [Indexed]
        public long StartTimestamp { get; set; }
        public long EndTimestamp { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string DataFileName { get; set; }

        public int BoardSerial { get; set; }

        [SQLite.Ignore]
        public string TextDisplay
        {
            get { return StartTime.ToShortDateString() + " " + StartTime.ToShortTimeString(); }
        }

        public Ride()
        {
        }

        public static Ride CreateNewRide()
        {
            var newRideGuid = Guid.Empty;
            var newDataFileName = String.Empty;
            var newDataFilePath = String.Empty;
            var dbRidesFound = 0;
            do
            {
                newRideGuid = Guid.NewGuid();
                newDataFileName = $"{newRideGuid.ToString().ToUpper()}.bin";
                newDataFilePath = Path.Combine(App.Current.LogsDirectory, newDataFileName);

                // Check a ride doens't exist in the database either.
                dbRidesFound = Database.Connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Ride WHERE id=?", newRideGuid);
            } while (File.Exists(newDataFilePath) == true || dbRidesFound > 0);
           

            var newRide = new Ride()
            {
                ID = newRideGuid,
                StartTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                StartTime = DateTime.Now,
                EndTime = DateTime.MaxValue,
                DataFileName = newDataFileName
            };
            newRide.Name = "Ride on " + newRide.TextDisplay;

            // Insert into DB.
            Database.Connection.Insert(newRide);

            return newRide;
        }

        public void Save()
        {
            Database.Connection.Update(this);
        }
    }
}
