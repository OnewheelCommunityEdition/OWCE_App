using System;
namespace OWCE.DependencyInterfaces
{
    public interface IWatch
    {
        void UpdateBatteryPercent(int percent);
        void UpdateSpeed(int speed);
        void UpdateVoltage(float voltage);
        void UpdateDistance(string distanceString);
        void ListenForWatchMessages(OWBoard board);
    }
}
