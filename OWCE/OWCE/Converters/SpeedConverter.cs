using System;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var outputValue = 0f;
            if (value is float metersPerSecond)
            {
                outputValue = ConvertSpeedValue(metersPerSecond, App.Current.MetricDisplay);
            }

            if (parameter == null)
            {
                return $"{outputValue:F1}"; // Math.Round(outputValue, 1);
            }

            if (parameter is string forDisplay && forDisplay == "DisplayUnits")
            {
                var unit = (App.Current.MetricDisplay ? "km/h" : "mph");
                return $"{outputValue:F1} {unit}";
            }

            return outputValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static float ConvertSpeedValue(float metersPerSecond, bool isMetric)
        {
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
