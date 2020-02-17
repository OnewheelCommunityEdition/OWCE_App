using System;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float distanceInMiles)
            {
                if (App.Current.MetricDisplay == false)
                {
                    return $"{distanceInMiles:F0}mi";
                }

                var distanceKilometers = distanceInMiles * 1.60934;
                return $"{distanceKilometers:F0}km";
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
