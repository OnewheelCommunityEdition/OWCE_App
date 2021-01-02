using System;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class RpmToSpeedConverter : IValueConverter
    {
        public const float TwoPi = (2f * (float)Math.PI);
        public const float RadConvert = (TwoPi / 60f);
        public const float MillimetersPerMinuteToMetersPerSecond = 0.00001666666667f;
        public RpmToSpeedConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int rpm) // && parameter is float wheelCircumfrence)
            {
                // TODO: Not use static wheel circumfrence. This converter is not currently being used for UI or reporting.
                float wheelCircumference = 917.66f;
                return ConvertSpeedValueFromRpm(rpm, wheelCircumference, App.Current.MetricDisplay);
            }

            return 0.0f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static float ConvertSpeedValueFromRpm(int rpm, float wheelCircumference, bool isMetric)
        {
            var metersPerSecond = wheelCircumference * (float)rpm * MillimetersPerMinuteToMetersPerSecond;

            if (isMetric)
            {
                return metersPerSecond * 3.6f; // kmph
            }
            else
            {
                return metersPerSecond * 2.23694f; // mph
            }
        }
    }
}
