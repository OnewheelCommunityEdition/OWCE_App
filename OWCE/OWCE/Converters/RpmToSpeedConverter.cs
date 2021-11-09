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
                // TODO: Not use static wheel circumfrence.
                return ConvertFromRpm(rpm);
            }

            return 0.0f;
        }

        public static float ConvertFromRpm(int rpm)
        {
            var metersPerSecond = 917.66f * rpm * MillimetersPerMinuteToMetersPerSecond;

            if (App.Current.MetricDisplay)
            {
                return metersPerSecond * 3.6f; // kmph
            }
            else
            {
                return metersPerSecond * 2.23694f; // mph
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
