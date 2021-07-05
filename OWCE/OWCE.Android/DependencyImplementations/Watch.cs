using OWCE.DependencyInterfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.Droid.DependencyImplementations.Watch))]

namespace OWCE.Droid.DependencyImplementations
{
    public class Watch : IWatch
    {
        public void BoardConnected()
        {
            // Implement when we have Android Watch support
        }

        public void UpdateBatteryPercent(int percent)
        {
            // Implement when we have Android Watch support
        }

        public void UpdateDistance(string distanceString)
        {
            // Implement when we have Android Watch support
        }

        public void UpdateSpeed(int speed)
        {
            // Implement when we have Android Watch support
        }

        public void UpdateVoltage(float voltage)
        {
            // Implement when we have Android Watch support
        }
    }
}
