using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
	public class UUIDToNameConverter : IValueConverter
    {
        Dictionary<string, string> uuidLookup = new Dictionary<string, string>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (uuidLookup.ContainsKey(stringValue))
                {
                    return uuidLookup[stringValue];
                }

                var actualName = stringValue.ToUpper() switch
                {
                    OWBoard.SerialNumberUUID => "SerialNumber",
                    OWBoard.RideModeUUID => "RideMode",
                    OWBoard.BatteryPercentUUID => "BatteryPercent",
                    OWBoard.BatteryLow5UUID => "BatteryLow5",
                    OWBoard.BatteryLow20UUID => "BatteryLow20",
                    OWBoard.BatterySerialUUID => "BatterySerial",
                    OWBoard.PitchUUID => "Pitch",
                    OWBoard.RollUUID => "Roll",
                    OWBoard.YawUUID => "Yaw",
                    OWBoard.TripOdometerUUID => "TripOdometer",
                    OWBoard.RpmUUID => "Rpm",
                    OWBoard.LightModeUUID => "LightMode",
                    OWBoard.LightsFrontUUID => "LightsFront",
                    OWBoard.LightsBackUUID => "LightsBack",
                    OWBoard.StatusErrorUUID => "StatusError",
                    OWBoard.TemperatureUUID => "Temperature",
                    OWBoard.FirmwareRevisionUUID => "FirmwareRevision",
                    OWBoard.CurrentAmpsUUID => "CurrentAmps",
                    OWBoard.TripAmpHoursUUID => "TripAmpHours",
                    OWBoard.TripRegenAmpHoursUUID => "TripRegenAmpHours",
                    OWBoard.BatteryTemperatureUUID => "BatteryTemperature",
                    OWBoard.BatteryVoltageUUID => "BatteryVoltage",
                    OWBoard.SafetyHeadroomUUID => "SafetyHeadroom",
                    OWBoard.HardwareRevisionUUID => "HardwareRevision",
                    OWBoard.LifetimeOdometerUUID => "LifetimeOdometer",
                    OWBoard.LifetimeAmpHoursUUID => "LifetimeAmpHours",
                    OWBoard.BatteryCellsUUID => "BatteryCells",
                    OWBoard.LastErrorCodeUUID => "LastErrorCode",
                    OWBoard.SerialReadUUID => "SerialRead",
                    OWBoard.SerialWriteUUID => "SerialWrite",
                    OWBoard.UNKNOWN1UUID => "UNKNOWN1",
                    OWBoard.RideTraitsUUID => "UNKNOWN2",
                    OWBoard.UNKNOWN3UUID => "UNKNOWN3",
                    OWBoard.UNKNOWN4UUID => "UNKNOWN4",
                    _ => "Unknown",
                };

                uuidLookup[stringValue] = actualName;

                return actualName;
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

