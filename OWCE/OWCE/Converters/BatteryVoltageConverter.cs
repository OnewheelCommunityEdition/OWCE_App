namespace OWCE.Converters
{
    using OWCE.Extensions;
    using OWCE.Spline;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xamarin.Forms;

    public class BatteryVoltageConverter : IValueConverter
    {
        private static Tuple<float, float>[] CBVoltageMapping = new Tuple<float, float>[]
        {
            new Tuple<float, float>(62.25f, 100f),
            new Tuple<float, float>(60.75f, 93f),
            new Tuple<float, float>(60f, 86f),
            new Tuple<float, float>(58.8f, 79f),
            new Tuple<float, float>(57.75f, 72f),
            new Tuple<float, float>(56.7f, 65f),
            new Tuple<float, float>(55.5f, 58f),
            new Tuple<float, float>(54.75f, 51f),
            new Tuple<float, float>(54f, 44f),
            new Tuple<float, float>(52.95f, 37f),
            new Tuple<float, float>(51.75f, 30f),
            new Tuple<float, float>(51f, 23f),
            new Tuple<float, float>(48f, 16f),
            new Tuple<float, float>(45f, 9f),
            new Tuple<float, float>(43.5f, 0f)
        };

        private static Lazy<List<Tuple<float, float>>> _voltageMap = new Lazy<List<Tuple<float, float>>>(() =>
        {
            float[] fxArray = CBVoltageMapping.Select(t => t.Item1).ToArray();
            float[] fyArray = CBVoltageMapping.Select(t => t.Item2).ToArray();

            float[] xs;
            float[] ys;
            CubicSpline.FitParametric(fxArray, fyArray, StepsPrecisionCount, out xs, out ys);

            SortPairInplace(xs, ys);

            var voltageMap = new List<Tuple<float, float>>();
            for (int i = 0; i < xs.Length; ++i)
            {
                voltageMap.Add(new Tuple<float, float>(xs[i], ys[i]));
            }

            return voltageMap;
        });

        private const int StepsPrecisionCount = 1000;

        public BatteryVoltageConverter()
        {
        }

        public static int GetBatteryPercentEstimate(float voltage)
        {
            return (int)(_voltageMap.Value.BinarySearchClosestBind(i => i.Item1, voltage).Item2);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float voltageValue)
            {
                int percentageValue = GetBatteryPercentEstimate(voltageValue);
                return $"{percentageValue}%";
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static void SortPairInplace<T>(T[] xArray, T[] yArray) where T : IComparable
        {
            var indexList = new List<Tuple<T, T>>();
            for (int i = 0; i < xArray.Length; ++i)
            {
                indexList.Add(new Tuple<T, T>(xArray[i], yArray[i]));
            }

            indexList = indexList.OrderBy(kv => kv.Item2).ToList();
            for (int i = 0; i < indexList.Count; ++i)
            {
                xArray[i] = indexList[i].Item1;
                yArray[i] = indexList[i].Item2;
            }
        }
    }
}
