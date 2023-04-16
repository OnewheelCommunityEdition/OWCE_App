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
            if (value is int turnCompensation)
                return Rescaler.RescaleValue(turnCompensation, -100.0f, 100.0f, 0.0f, 10.0f);
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float turnCompensation)
                return (int)Math.Round(Rescaler.RescaleValue(turnCompensation, 0.0f, 10.0f, -100.0f, 100.0f));

            return 0;
        }
    }
}
