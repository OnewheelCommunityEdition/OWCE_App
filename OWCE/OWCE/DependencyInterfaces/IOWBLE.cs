using System;
using System.Threading;
using System.Threading.Tasks;

namespace OWCE.DependencyInterfaces
{
    public interface IOWBLE
    {
        public bool IsScanning { get; }
        void StartScanning();
        void StopScanning();
        bool ReadyToScan();
        void Shutdown();
        void RequestRSSIUpdate();

        Task<bool> Connect(OWBaseBoard board, CancellationToken cancellationToken);
        Task Disconnect();

        Action<String> ErrorOccurred { get; set; }

        Action<OWBaseBoard> BoardDiscovered { get; set; }
        Action<BluetoothState> BLEStateChanged { get; set; }
        Action<string, byte[]> BoardValueChanged { get; set; }
        Action<int> RSSIUpdated { get; set; }
        Action BoardDisconnected { get; set; }
        Action BoardReconnecting { get; set; }
        Action BoardReconnected { get; set; }


        Task<byte[]> ReadValue(string characteristicGuid, bool important = false);
        Task<byte[]> WriteValue(string characteristicGuid, byte[] data, bool important = false);
        Task SubscribeValue(string characteristicGuid, bool important = false);
        Task UnsubscribeValue(string characteristicGuid, bool important = false);
    }
}
