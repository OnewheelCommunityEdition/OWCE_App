using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class AngleOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int angleOffset)
                return -(angleOffset * .05f);
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float angleOffset)
                return (int) Math.Round(-angleOffset / 0.05f);
            return 0;
        }
    }
}
