using System;
namespace OWCE.DependencyInterfaces
{
    // Handles communication with the watch.
    // Platform-specific implementations are expected to implement
    // this interface via DependencyService
    public interface IWatch
    {
        void UpdateBatteryPercent(int percent);
        void UpdateSpeed(int speed);
        void UpdateVoltage(float voltage);
        void UpdateDistance(string distanceString);
        void ListenForWatchMessages(OWBoard board);
    }
}
