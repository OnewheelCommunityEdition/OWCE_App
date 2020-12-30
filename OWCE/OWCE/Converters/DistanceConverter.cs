using System;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = "F0";

            if (parameter is string formatString)
            {
                format = formatString;
            }

            if (value is float distanceInMilesAsFloat)
            {
                if (App.Current.MetricDisplay == false)
                {
                    return $"{distanceInMilesAsFloat.ToString(format)} mi";
                }

                var distanceKilometers = distanceInMilesAsFloat * 1.60934;
                return $"{distanceKilometers.ToString(format)} km";
            }
            else if (value is int distanceInMilesAsInt)
            {
                if (App.Current.MetricDisplay == false)
                {
                    return $"{distanceInMilesAsInt.ToString(format)} mi";
                }

                var distanceKilometers = (double)distanceInMilesAsInt * 1.60934;
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
