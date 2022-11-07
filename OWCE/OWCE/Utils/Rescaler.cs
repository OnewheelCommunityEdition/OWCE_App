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
    }
}
