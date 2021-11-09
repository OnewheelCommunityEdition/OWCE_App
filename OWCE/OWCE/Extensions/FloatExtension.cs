using System;
namespace System
{
    public static class FloatExtension
    {
        private static double _precision = 0.0000001;

        public static bool AlmostEqualTo(this float value1, float value2)
        {
            return Math.Abs(value1 - value2) < _precision;
        }

        public static bool AlmostEqualTo(this float value1, float value2, double precision)
        {
            return Math.Abs(value1 - value2) < precision;
        }
    }
}
