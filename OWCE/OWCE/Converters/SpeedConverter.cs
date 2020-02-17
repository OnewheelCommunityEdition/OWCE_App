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
                if (App.Current.MetricDisplay)
                {
                    outputValue = metersPerSecond * 3.6f; // kmph
                }
                else
                {
                    outputValue = metersPerSecond * 2.23694f; // mph
                }
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
    }
}
