using System;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;
using CoreBluetooth;
using Foundation;
using CoreFoundation;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

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
            Debug.WriteLine("StartScanning");

            _centralManager.ScanForPeripherals(new CBUUID[] { OWBoard.ServiceUUID.ToCBUUID() }, new PeripheralScanningOptions { AllowDuplicatesKey = true });

            await Task.Delay(timeout * 1000);

            StopScanning();
        }

        public void StopScanning()
        {
            Debug.WriteLine("StopScanning");
            if (_centralManager.IsScanning)
            {
                _centralManager.StopScan();
            }
        }

        CBService _service;

        [Export("peripheral:didDiscoverServices:")]
        public void DiscoveredService(CBPeripheral peripheral, NSError error)
        {
            Debug.WriteLine($"Peripheral_DiscoveredService: {peripheral.Name}");

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

        Dictionary<CBUUID, CBCharacteristic> _characteristics = new Dictionary<CBUUID, CBCharacteristic>();

        [Export("peripheral:didDiscoverCharacteristicsForService:error:")]
        public void DiscoveredCharacteristic(CBPeripheral peripheral, CBService service, NSError error)
        {
            Debug.WriteLine("Peripheral_DiscoveredCharacteristic");
            //var cbuuid = CBUUID.FromString(characteristicGuid);
            foreach (var characteristic in _service.Characteristics)
            {
                if (_characteristics.ContainsKey(characteristic.UUID))
                {
                    _characteristics[characteristic.UUID] = characteristic;
                }
                else
                {
                    _characteristics.Add(characteristic.UUID, characteristic);
                }
            }

            BoardConnected?.Invoke(_board);
        }

        [Export("peripheral:didUpdateValueForCharacteristic:error:")]
        public void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
#if DEBUG
            string name =  OWBoard.GetNameFromUUID(characteristic.UUID.ToString());
            Debug.WriteLine($"Peripheral_UpdatedCharacterteristicValue - {name}");
#endif

            if (_characteristics.ContainsKey(characteristic.UUID) == false)
            {
                Debug.WriteLine($"ERROR: Peripheral missing ({characteristic.UUID})");
                return;
            }


            if (_readQueue.ContainsKey(characteristic.UUID) || _notifyList.Contains(characteristic.UUID))
            {
                byte[] dataBytes = null;

                if (characteristic.Value != null)
                {
                    var data = characteristic.Value;
                    dataBytes = new byte[data.Length];
                    System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));
                }

                if (_notifyList.Contains(characteristic.UUID))
                {
                    BoardValueChanged?.Invoke(characteristic.UUID.ToString(), dataBytes);
                }

                if (_readQueue.ContainsKey(characteristic.UUID))
                {
                    var task = _readQueue[characteristic.UUID];
                    _readQueue.Remove(characteristic.UUID);
                    task.SetResult(dataBytes);
                }
            }
        }

        [Export("peripheral:didWriteValueForCharacteristic:error:")]
        public void WroteCharacteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            Debug.WriteLine("Peripheral_WroteCharacteristicValue");


            if (_characteristics.ContainsKey(characteristic.UUID) == false)
            {
                return;
            }

            if (_writeQueue.ContainsKey(characteristic.UUID) == false)
            {
                return;
            }

            var data = characteristic.Value;
            byte[] dataBytes;
            if (data != null)
            {
                dataBytes = new byte[data.Length];
                System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));
            }
            else
            {
                dataBytes = null;
            }

            var task = _writeQueue[characteristic.UUID];
            _writeQueue.Remove(characteristic.UUID);
            task.SetResult(dataBytes);
        }

        #region CBCentralManager_Delegate
        [Export("centralManager:didConnectPeripheral:")]
        public void CentralManager_ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            Debug.WriteLine("CentralManager_ConnectedPeripheral: " + peripheral.Name);
            _connectionCompletionSource.SetResult(true);

            var services = _peripheral.Services;
            _peripheral.DiscoverServices(new CBUUID[] { OWBoard.ServiceUUID.ToCBUUID() });

            //this.BoardConnected?.Invoke(_board);
        }


        [Export("centralManager:didDiscoverPeripheral:advertisementData:RSSI:")]
        public void CentralManager_DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            Debug.WriteLine("CentralManager_DiscoveredPeripheral: "+ peripheral.Name);

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
            Debug.WriteLine("CentralManager_DisconnectedPeripheral");
        }

        [Export("centralManager:didFailToConnectPeripheral:error:")]
        public void CentralManager_FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Debug.WriteLine("CentralManager_FailedToConnectPeripheral");
        }

        [Export("centralManager:didRetrieveConnectedPeripherals:")]
        public void CentralManager_RetrievedConnectedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            Debug.WriteLine("CentralManager_RetrievedConnectedPeripherals");

        }

        [Export("centralManager:didRetrievePeripherals:")]
        public void CentralManager_RetrievedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            Debug.WriteLine("CentralManager_RetrievedPeripherals");
        }

        public void UpdatedState(CBCentralManager central)
        {
            Debug.WriteLine("CentralManager_UpdatedState: " + central.State);
        }

        private void CentralManager_WillRestoreState(object sender, CBWillRestoreEventArgs e)
        {
            Debug.WriteLine("CentralManager_WillRestoreState");

        }
        #endregion

        TaskCompletionSource<bool> _connectionCompletionSource = null;
        OWBoard _board;

        #region IOWBLE
        public Action<BluetoothState> BLEStateChanged { get; set; }
        public Action<OWBoard> BoardDiscovered { get; set; }
        public Action<OWBoard> BoardConnected { get; set; }
        public Action<string, byte[]> BoardValueChanged { get; set; }

        public Task<bool> Connect(OWBoard board)
        {
            _connectionCompletionSource = new TaskCompletionSource<bool>();

            if (board.NativePeripheral is CBPeripheral peripheral)
            {
                _board = board;
                _peripheral = peripheral;
                _peripheral.WeakDelegate = this;

                var options = new PeripheralConnectionOptions()
                {
                    NotifyOnDisconnection = true,
#if __IOS__
                    NotifyOnConnection = true,
                    NotifyOnNotification = true,
#endif
                };

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
            Debug.WriteLine("Disconnect");
            _centralManager.CancelPeripheralConnection(_peripheral);
            return Task.CompletedTask;
        }

        Dictionary<CBUUID, TaskCompletionSource<byte[]>> _readQueue = new Dictionary<CBUUID, TaskCompletionSource<byte[]>>();
        Dictionary<CBUUID, TaskCompletionSource<byte[]>> _writeQueue = new Dictionary<CBUUID, TaskCompletionSource<byte[]>>();
        List<CBUUID> _notifyList = new List<CBUUID>();



        public Task<byte[]> ReadValue(string characteristicGuid, bool important = false)
        {
            var cbuuid = CBUUID.FromString(characteristicGuid);
            
            // TODO: Check for connected devices?
            if (_characteristics.ContainsKey(cbuuid) == false)
            {
                // TODO Error?
                return null;
            }

            // Already awaiting it.
            if (_readQueue.ContainsKey(cbuuid))
            {
                return _readQueue[cbuuid].Task;
            }

            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            if (important)
            {
                // TODO: Put this at the start of the queue.
                _readQueue.Add(cbuuid, taskCompletionSource);
            }
            else
            {
                _readQueue.Add(cbuuid, taskCompletionSource);
            }


            _peripheral.ReadValue(_characteristics[cbuuid]);

            return taskCompletionSource.Task;
        }

        public Task<byte[]> WriteValue(string characteristicGuid, byte[] data, bool important = false)
        {
            var cbuuid = CBUUID.FromString(characteristicGuid);

            // TODO: Check for connected devices?
            if (_characteristics.ContainsKey(cbuuid) == false)
            {
                // TODO Error?
                return null;
            }

            var characteristic = _characteristics[cbuuid];
            var nsData = NSData.FromArray(data);

            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            if (important)
            {
                // TODO: Put this at the start of the queue.
                _writeQueue.Add(cbuuid, taskCompletionSource);
            }
            else
            {
                _writeQueue.Add(cbuuid, taskCompletionSource);
            }

            _peripheral.WriteValue(nsData, characteristic, CBCharacteristicWriteType.WithResponse);

            return taskCompletionSource.Task;
        }

        public Task SubscribeValue(string characteristicGuid, bool important = false)
        {
            var cbuuid = CBUUID.FromString(characteristicGuid);

            // TODO: Check for connected devices?
            if (_characteristics.ContainsKey(cbuuid) == false)
            {
                // TODO Error?
                return null;
            }
            if (_notifyList.Contains(cbuuid) == false)
            {
                _notifyList.Add(cbuuid);
            }
            _peripheral.SetNotifyValue(true, _characteristics[cbuuid]);

            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public Task UnsubscribeValue(string characteristicGuid, bool important = false)
        {
            var cbuuid = CBUUID.FromString(characteristicGuid);

            // TODO: Check for connected devices?
            if (_characteristics.ContainsKey(cbuuid) == false)
            {
                // TODO Error?
                return null;
            }
            _notifyList.RemoveAll((c) => c.Equals(cbuuid));
            _peripheral.SetNotifyValue(false, _characteristics[cbuuid]);

            
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        public bool BluetoothEnabled()
        {
            return _centralManager.State == CBCentralManagerState.PoweredOn;
        }
        #endregion
    }
}
