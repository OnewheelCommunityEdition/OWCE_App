using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class TempConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int tempCelsius)
            {
                if (App.Current.MetricDisplay)
                {
                    return $"{tempCelsius}°C";
                }

                var tempFahrenheit = (int)(tempCelsius * 1.8f) + 32;
                return $"{tempFahrenheit}°F";
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
