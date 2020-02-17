using System;
using System.Threading.Tasks;

namespace OWCE.DependencyInterfaces
{
    public interface IOWBLE
    {
        Action<BluetoothState> BLEStateChanged { get; set; }
        Action<OWBaseBoard> BoardDiscovered { get; set; }
        Action<OWBoard> BoardConnected { get; set; }
        Action<string, byte[]> BoardValueChanged { get; set; }

        Task<bool> Connect(OWBaseBoard board);
        Task Disconnect();
        Task StartScanning(int timeout = 15);
        void StopScanning();
        Task<byte[]> ReadValue(string characteristicGuid, bool important = false);
        Task<byte[]> WriteValue(string characteristicGuid, byte[] data, bool important = false);
        Task SubscribeValue(string characteristicGuid, bool important = false);
        Task UnsubscribeValue(string characteristicGuid, bool important = false);

        bool BluetoothEnabled();
    }
}
