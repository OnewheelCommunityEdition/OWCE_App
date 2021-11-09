using System;
using System.Globalization;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class SelectedButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                if (isSelected)
                {
                    return (Style)Application.Current.Resources["SelectedRoundButtonStyle"];
                }
            }

            return (Style)Application.Current.Resources["RoundButtonStyle"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
