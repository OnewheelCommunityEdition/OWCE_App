using OWCE.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class TurnCompensationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Rescaler.RescaleValue((float)value, -100.0f, 100.0f, 0.0f, 10.0f);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Rescaler.RescaleValue((float)value, 0.0f, 10.0f, -100.0f, 100.0f);
        }
    }
}
