using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class TemperatureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tempCelsius = System.Convert.ToInt32(value);

            if (App.Current.MetricDisplay)
            {
                return $"{tempCelsius:F0}°C";
            }

            var tempFahrenheit = (int)(tempCelsius * 1.8f) + 32;
            return $"{tempFahrenheit:F0}°F";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
