using System;
using System.Threading.Tasks;

namespace OWCE.DependencyInterfaces
{

    public interface IOWBLE
    {
        Action<BluetoothState> BLEStateChanged { get; set; }
        Action<OWBoard> BoardDiscovered { get; set; }
        Action<OWBoard> BoardConnected { get; set; }

        Task<bool> Connect(OWBoard board);
        Task Disconnect();
        Task StartScanning(int timeout = 15);
        void StopScanning();
        Task<byte[]> ReadValue(string characteristicGuid, bool important = false);
        Task<byte[]> WriteValue(string characteristicGuid, byte[] data, bool important = false);
        Task SubscribeValue(string characteristicGuid, bool important = false);
        Task UnsubscribeValue(string characteristicGuid, bool important = false);
    }
}
