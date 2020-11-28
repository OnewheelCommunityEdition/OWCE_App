using System;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class IsBoardTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OWBoardType boardType && parameter is string requiredBoardType)
            {
                var boardTypeString = boardType.ToString();
                if (boardTypeString.Equals(requiredBoardType, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
