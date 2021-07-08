using System;
using System.Collections.Generic;
using OWCE.Converters;
using OWCE.DependencyInterfaces;
using WatchConnectivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.iOS.DependencyImplementations.Watch))]

namespace OWCE.iOS.DependencyImplementations
{
    // Implementation of IWatch on iOS.
    // Messages are received on InterfaceController.cs on the watch side.
    public class Watch : IWatch
    {
        private OWBoard _board;

        public void BoardConnected()
        {
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "MessagePhone", "Connected" } });
        }

        void IWatch.UpdateBatteryPercent(int percent)
        {
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "BatteryPercent", percent } });
        }

        void IWatch.UpdateDistance(string distanceString)
        {
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "Distance", distanceString } });
        }

        void IWatch.UpdateSpeed(int speed)
        {
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "Speed", speed} });
        }

        void IWatch.UpdateVoltage(float voltage)
        {
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "Voltage", voltage} });
        }

        void IWatch.ListenForWatchMessages(OWBoard board)
        {
            _board = board;
            WCSessionManager.SharedManager.MessageReceived += DidReceiveMessage;
        }

        public void DidReceiveMessage(WCSession session, Dictionary<string, object> message)
        {
            if (message.ContainsKey("WatchAppAwake"))
            {
                if (_board == null)
                {
                    Console.WriteLine("Board not initialized yet. Returning");
                    return;
                }
                // Watch just woke up -- send all current data to bring
                // the watch up to speed
                SendAllBoardData(_board);
            }
        }

        private void SendAllBoardData(OWBoard board)
        {
            try
            {
                int rpm = board.RPM;
                int speedMph = (int)RpmToSpeedConverter.ConvertFromRpm(rpm);
                ushort tripOdometer = board.TripOdometer;
                string distanceString = RotationsToDistanceConverter.ConvertRotationsToDistance(tripOdometer);

                WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                        { "BatteryPercent", board.BatteryPercent },
                        { "Speed", speedMph},
                        { "Voltage", board.BatteryVoltage},
                        { "SpeedUnitsLabel", App.Current.MetricDisplay ? "km/h" : "mph"},
                        { "Distance", distanceString}
                    });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Processing Message: {ex.Message}");
            }
        }
    }
}
