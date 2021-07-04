using System;
namespace OWCE.DependencyInterfaces
{
    public interface IWatch
    {
        void BoardConnected();
        void UpdateBatteryPercent(int percent);
        void UpdateSpeed(int speed);
        void UpdateVoltage(float voltage);
    }
}
