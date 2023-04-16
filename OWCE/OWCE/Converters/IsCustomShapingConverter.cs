using System;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class IsCustomShapingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ushort rideMode)
            {
                Console.WriteLine("Ride Mode", rideMode);
                return rideMode == 9;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
