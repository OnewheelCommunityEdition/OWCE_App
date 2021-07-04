using System;
using System.Collections.Generic;
using BigTed;
using OWCE.DependencyInterfaces;
using WatchConnectivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.iOS.DependencyImplementations.Watch))]

namespace OWCE.iOS.DependencyImplementations
{
    public class Watch : IWatch
    {
        public void BoardConnected()
        {
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "MessagePhone", "Connected" } });
        }

        void IWatch.UpdateBatteryPercent(int percent)
        {
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "BatteryPercent", percent } });
        }

        void IWatch.UpdateSpeed(int speed)
        {
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "Speed", speed} });
        }

        void IWatch.UpdateVoltage(float voltage)
        {
            WCSessionManager.SharedManager.SendMessage(new Dictionary<string, object>() {
                { "Voltage", voltage} });
        }
    }
}
