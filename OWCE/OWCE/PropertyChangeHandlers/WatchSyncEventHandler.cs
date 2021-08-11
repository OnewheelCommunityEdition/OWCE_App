using System;
using System.Collections.Generic;
using System.ComponentModel;
using OWCE.Converters;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;

namespace OWCE.PropertyChangeHandlers
{
    public class WatchSyncEventHandler
    {
        private static HashSet<String> PropertiesToWatch = new HashSet<string> { "BatteryVoltage", "RPM", "TripOdometer" };

        public static void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (!PropertiesToWatch.Contains(e.PropertyName)) { return; }

                IWatch watchService = DependencyService.Get<IWatch>();
                if (e.PropertyName.Equals("BatteryVoltage"))
                {
                    float voltage = (sender as OWBoard).BatteryVoltage;
                    watchService.UpdateVoltage(voltage);

                    // For Quart
                    //double pct = 99.9 / (0.8 + Math.Pow(1.28, 54 - voltage)) - 9;
                    //watchService.UpdateBatteryPercent((int)pct);
                }
                if (e.PropertyName.Equals("RPM"))
                {
                    int rpm = (sender as OWBoard).RPM;
                    int speedMph = (int)RpmToSpeedConverter.ConvertFromRpm(rpm);
                    watchService.UpdateSpeed(speedMph);
                }
                if (e.PropertyName.Equals("BatteryPercent"))
                {
                    int batteryPercent = (sender as OWBoard).BatteryPercent;
                    watchService.UpdateBatteryPercent(batteryPercent);
                }
                if (e.PropertyName.Equals("TripOdometer"))
                {
                    ushort tripOdometer = (sender as OWBoard).TripOdometer;
                    string tripDescription = RotationsToDistanceConverter.ConvertRotationsToDistance(tripOdometer);
                    watchService.UpdateDistance(tripDescription);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Handling Watch Property Change: {ex.Message}");
                //(sender as OWBoard).ErrorMessage = $"Exception Handling Watch Property Change: {ex.Message}";
            }

        }
    }
}
