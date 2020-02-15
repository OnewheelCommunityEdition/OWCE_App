using System;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class DivideByTwo : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int valueInt)
            {
                return valueInt / 2;
            }
            else if (value is float valueFloat)
            {
                return valueFloat * 0.5f;
            }
            else if (value is double valueDouble)
            {
                return valueDouble * 0.5;
            }

            // Fall back to return what the value was.
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
