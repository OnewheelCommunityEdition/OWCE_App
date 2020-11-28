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
                var format = "F0";

                if (parameter is string formatString)
                {
                    format = formatString;
                }

                if (App.Current.MetricDisplay == false)
                {
                    return $"{distanceInMiles.ToString(format)} mi";
                }

                var distanceKilometers = distanceInMiles * 1.60934;
                return $"{distanceKilometers.ToString(format)} km";
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
