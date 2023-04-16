using System;
using System.Collections.Generic;
using System.Text;

namespace OWCE.Utils
{
    public class Rescaler
    {
        public static float RescaleValue(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (((toMax - toMin) / (fromMax - fromMin)) * (value - fromMin));
        }

        public static int TwosComplement(byte value)
        {
            // If a positive value, return it
            if ((value & 0x80) == 0)
                return value;

            // Otherwise perform the 2's complement math on the value
            return (byte)(~(value - 0x01)) * -1;
        }

        public static byte TwosComplement(int value)
        {
            // If a positive value, return it
            if ((value & 0x80) == 0)
                return (byte)value;

            // Otherwise perform the 2's complement math on the value
            return (byte)(256 + value);
        }
    }
}
