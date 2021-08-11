using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Converters
{
    public class RotationsToDistanceConverter : IValueConverter
    {
        public const float TwoPi = (2f * (float)Math.PI);
        public const float RadConvert = (TwoPi / 60f);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ushort rotations)
            {
                return ConvertRotationsToDistance(rotations); 
            }

            return 0.0f;
        }

        public static string ConvertRotationsToDistance(ushort rotations)
        {
            // TODO: Not use static wheel circumfrence.
            var kilometers = 917.66f * rotations * 0.001f * 0.001f;

            if (App.Current.MetricDisplay)
            {
                return $"{kilometers.ToString("N1")} km"; // kmph
            }
            else
            {
                return $"{UnitConverters.KilometersToMiles(kilometers).ToString("N1")} mi";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
