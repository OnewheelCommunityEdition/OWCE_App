using System;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;
using CoreBluetooth;
using Foundation;
using CoreFoundation;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

#if __IOS__
using OWCE.iOS.Extensions;
[assembly: Dependency(typeof(OWCE.iOS.DependencyImplementations.OWBLE))]
namespace OWCE.iOS.DependencyImplementations
#elif __MACOS__
using OWCE.MacOS.Extensions;
[assembly: Dependency(typeof(OWCE.MacOS.DependencyImplementations.OWBLE))]
namespace OWCE.MacOS.DependencyImplementations
#endif
{
    public class OWBLE : NSObject, IOWBLE, ICBCentralManagerDelegate, ICBPeripheralDelegate
    {
        CBCentralManager _centralManager;
        CBPeripheral _peripheral;
        DispatchQueue _dispatchQueue;

        public OWBLE()
        {
            _dispatchQueue = new DispatchQueue("CBCentralManager_Queue");
            _centralManager = new CBCentralManager(this, _dispatchQueue);
        }

        public async Task StartScanning(int timeout = 15)
        {
            Console.WriteLine("StartScanning");

            _centralManager.ScanForPeripherals(new CBUUID[] { OWBoard.ServiceUUID.ToCBUUID() }, new PeripheralScanningOptions { AllowDuplicatesKey = true });

            await Task.Delay(timeout * 1000);

            StopScanning();
        }

        public void StopScanning()
        {
            Console.WriteLine("StopScanning");
            if (_centralManager.IsScanning)
            {
                _centralManager.StopScan();
            }
        }

        CBService _service;

        [Export("peripheral:didDiscoverServices:")]
        public void DiscoveredService(CBPeripheral peripheral, NSError error)
        {
            Console.WriteLine("Peripheral_DiscoveredService: " + peripheral.Name);

            CBService foundService = null;

            foreach (var service in peripheral.Services)
            {
                if (service.UUID == OWBoard.ServiceUUID.ToCBUUID())
                {
                    foundService = service;
                    break;
                }
            }

            if (foundService != null)
            {
                _service = foundService;
                _peripheral.DiscoverCharacteristics(_service);
            }
        }

        Dictionary<Guid, CBCharacteristic> _characteristics = new Dictionary<Guid, CBCharacteristic>();

        [Export("peripheral:didDiscoverCharacteristicsForService:error:")]
        public void DiscoveredCharacteristic(CBPeripheral peripheral, CBService service, NSError error)
        {
            Console.WriteLine("Peripheral_DiscoveredCharacteristic");

            foreach (var characteristic in _service.Characteristics)
            {
                var guid = characteristic.UUID.ToGuid();
                if (_characteristics.ContainsKey(guid))
                {
                    _characteristics[guid] = characteristic;
                }
                else
                {
                    _characteristics.Add(guid, characteristic);
                }
            }

            BoardConnected?.Invoke(_board);
        }

        [Export("peripheral:didUpdateValueForCharacteristic:error:")]
        public void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            Console.WriteLine("Peripheral_UpdatedCharacterteristicValue");

        }

        [Export("peripheral:didWriteValueForCharacteristic:error:")]
        public void WroteCharacteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            Console.WriteLine("Peripheral_WroteCharacteristicValue");

        }

        #region CBCentralManager_Delegate
        [Export("centralManager:didConnectPeripheral:")]
        public void CentralManager_ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            Console.WriteLine("CentralManager_ConnectedPeripheral: " + peripheral.Name);
            _connectionCompletionSource.SetResult(true);

            var services = _peripheral.Services;
            _peripheral.DiscoverServices(new CBUUID[] { OWBoard.ServiceUUID.ToCBUUID() });

            //this.BoardConnected?.Invoke(_board);
        }


        [Export("centralManager:didDiscoverPeripheral:advertisementData:RSSI:")]
        public void CentralManager_DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            Console.WriteLine("CentralManager_DiscoveredPeripheral: "+ peripheral.Name);

            OWBoard board = new OWBoard()
            {
                ID = peripheral.Identifier.ToString(),
                Name = peripheral.Name ?? "Onewheel",
                IsAvailable = true,
                NativePeripheral = peripheral,
            };

            BoardDiscovered?.Invoke(board);
        }

        [Export("centralManager:didDisconnectPeripheral:error:")]
        public void CentralManager_DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Console.WriteLine("CentralManager_DisconnectedPeripheral");
        }

        [Export("centralManager:didFailToConnectPeripheral:error:")]
        public void CentralManager_FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Console.WriteLine("CentralManager_FailedToConnectPeripheral");
        }

        [Export("centralManager:didRetrieveConnectedPeripherals:")]
        public void CentralManager_RetrievedConnectedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            Console.WriteLine("CentralManager_RetrievedConnectedPeripherals");

        }

        [Export("centralManager:didRetrievePeripherals:")]
        public void CentralManager_RetrievedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            Console.WriteLine("CentralManager_RetrievedPeripherals");
        }

        public void UpdatedState(CBCentralManager central)
        {
            Console.WriteLine("CentralManager_UpdatedState: " + central.State);
        }

        private void CentralManager_WillRestoreState(object sender, CBWillRestoreEventArgs e)
        {
            Console.WriteLine("CentralManager_WillRestoreState");

        }
        #endregion

        TaskCompletionSource<bool> _connectionCompletionSource = null;
        OWBoard _board;

        #region IOWBLE
        public Action<BluetoothState> BLEStateChanged { get; set; }
        public Action<OWBoard> BoardDiscovered { get; set; }
        public Action<OWBoard> BoardConnected { get; set; }

        public Task<bool> Connect(OWBoard board)
        {
            _connectionCompletionSource = new TaskCompletionSource<bool>();

            if (board.NativePeripheral is CBPeripheral peripheral)
            {
                _board = board;
                _peripheral = peripheral;
                _peripheral.WeakDelegate = this;

                var options = new PeripheralConnectionOptions();
                options.NotifyOnConnection = true;
                options.NotifyOnDisconnection = true;
                options.NotifyOnNotification = true;

                _centralManager.ConnectPeripheral(peripheral, options);
            }
            else
            {
                // TODO Alert.
            }

            return _connectionCompletionSource.Task;

        }

        public Task Disconnect()
        {
            Console.WriteLine("Disconnect");
            _centralManager.CancelPeripheralConnection(_peripheral);
            return Task.CompletedTask;
        }


        public Task<byte[]> ReadValue(string characteristicGuid, bool important = false)
        {


            //_peripheral.read
            throw new NotImplementedException();
        }

        public Task<byte[]> WriteValue(string characteristicGuid, byte[] data, bool important = false)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeValue(string characteristicGuid, bool important = false)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeValue(string characteristicGuid, bool important = false)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
