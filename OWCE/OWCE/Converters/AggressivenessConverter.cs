using OWCE.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class AggressivenessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int aggressiveness)
                return Rescaler.RescaleValue(aggressiveness, -80.0f, 127.0f, 0.0f, 10.0f);

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float aggressiveness)
                return (int) Math.Round(Rescaler.RescaleValue(aggressiveness, 0.0f, 10.0f, -80.0f, 127.0f));

            return 0;
        }
    }
}
