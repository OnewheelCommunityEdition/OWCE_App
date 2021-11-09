using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class BatteryCellsToTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Dictionary<uint, uint> batteryCells)
            {
                var sb = new StringBuilder();
                foreach (var cell in batteryCells.Keys.OrderBy(k => k))
                {
                    double voltage = batteryCells[cell] / 50.0;
                    sb.AppendLine($"{cell}: {batteryCells[cell]} ({voltage:N2}V)");
                }

                return sb.ToString();
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
