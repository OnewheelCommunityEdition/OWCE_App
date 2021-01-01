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
using System.ComponentModel;
using UIKit;
using System.Threading;

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
    public class OWBLE : NSObject, INotifyPropertyChanged, IOWBLE, ICBCentralManagerDelegate, ICBPeripheralDelegate
    {
        DispatchQueue _dispatchQueue;
        CBCentralManager _centralManager;
        CBPeripheral _peripheral;
        CBService _service;
        bool _updatingRSSI = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public Action<OWBaseBoard> BoardDiscovered { get; set; }
        public Action<String> ErrorOccurred { get; set; }
        TaskCompletionSource<bool> _connectionCompletionSource = null;
        TaskCompletionSource<bool> _disconnectionCompletionSource = null;

        OWBaseBoard _board;
        public Action<BluetoothState> BLEStateChanged { get; set; }
        public Action<OWBoard> BoardConnected { get; set; }
        public Action<string, byte[]> BoardValueChanged { get; set; }
        public Action<int> RSSIUpdated { get; set; }
        public Action BoardDisconnected { get; set; }
        public Action BoardReconnecting { get; set; }
        public Action BoardReconnected { get; set; }
        Dictionary<CBUUID, TaskCompletionSource<byte[]>> _readQueue = new Dictionary<CBUUID, TaskCompletionSource<byte[]>>();
        List<CharacteristicValueRequest> _writeQueue = new List<CharacteristicValueRequest>();
        List<CBUUID> _notifyList = new List<CBUUID>();
        bool _requestingDisconnect = false;
        bool _reconnecting = false;



        bool _isScanning = false;
        public bool IsScanning
        {
            get
            {
                return _isScanning;
            }
            set
            {
                if (_isScanning == value)
                    return;

                _isScanning = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public OWBLE()
        {
            //_dispatchQueue = new DispatchQueue("CBCentralManager_Queue");
            //_centralManager = new CBCentralManager(this, _dispatchQueue);
        }


        public void Shutdown()
        {
            StopScanning();

            if (_service != null)
            {
                _service.Dispose();
                _service = null;
            }

            if (_peripheral != null)
            {
                _peripheral.Dispose();
                _peripheral = null;
            }

            if (_centralManager != null)
            {
                _centralManager.Dispose();
                _centralManager = null;
            }

            if (_dispatchQueue != null)
            {
                _dispatchQueue.Dispose();
                _dispatchQueue = null;
            }
        }

        public void RequestRSSIUpdate()
        {
            if (_updatingRSSI)
            {
                return;
            }

            _updatingRSSI = true;
            _peripheral?.ReadRSSI();            
        }

        [Export("peripheral:didReadRSSI:error:")]
        public void RssiRead(CBPeripheral peripheral, NSNumber rssi, NSError error)
        {
            if (error == null)
            {
                RSSIUpdated?.Invoke(rssi.Int32Value);
                _updatingRSSI = false;
            }
        }

        public void StartScanning()
        {
            Debug.WriteLine("StartScanning");

            // If this is the first attempt to scan we create the centralManager.
            // This will prompt the user for the "OWCE wants to use bluetooth" prompt.
            if (_centralManager == null)
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
               {
                   _dispatchQueue = new DispatchQueue("CBCentralManager_Scanner_Queue");
                   _centralManager = new CBCentralManager(this, _dispatchQueue); // Prompt displays here, but not a blocking call.
                });
            }
            else
            {
                if (_centralManager.State == CBCentralManagerState.PoweredOn)
                {
                    DoActualScan();
                }
                else if (_centralManager.State == CBCentralManagerState.PoweredOff)
                {
                    throw new Exception("Bluetooth is currently turned off.");
                }
                else if (_centralManager.State == CBCentralManagerState.Unauthorized) // User has rejected authorisation.
                {
                    throw new Exception("Bluetooth permissions is disabled for OWCE.");
                }
                else if (_centralManager.State == CBCentralManagerState.Resetting)
                {
                    throw new Exception("Bluetooth scanning is not supported on this device.");
                }
                else if (_centralManager.State == CBCentralManagerState.Unsupported)
                {
                    throw new Exception("Bluetooth scanning is not supported on this device.");
                }
            }
        }

        public void StopScanning()
        {
            Debug.WriteLine("StopScanning");
            if (_centralManager != null && _centralManager.IsScanning)
            {
                _centralManager.StopScan();
            }
            IsScanning = false;
        }

        private void DoActualScan()
        {
            if (_centralManager == null || _centralManager.IsScanning)
            {
                return;
            }

            if (_centralManager.State == CBCentralManagerState.PoweredOn)
            {
                _centralManager.ScanForPeripherals(new CBUUID[] { OWBoard.ServiceUUID.ToCBUUID() }, new PeripheralScanningOptions { AllowDuplicatesKey = true });
                IsScanning = true;
            }
        }

        public Task<bool> ReadyToScan()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 1))
            {
                // NotDetermined before prompt.

                if (CBCentralManager.Authorization == CBManagerAuthorization.AllowedAlways)
                {
                    return Task.FromResult<bool>(true);
                }
            }

            return Task.FromResult<bool>(false);
        }

        Dictionary<CBUUID, CBCharacteristic> _characteristics = new Dictionary<CBUUID, CBCharacteristic>();

        #region ICBPeripheralDelegate
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

            _connectionCompletionSource.SetResult(true);

            //BoardConnected?.Invoke(null);
            //BoardConnected?.Invoke(new OWBoard(_board));
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

                    // Don't reveres for serial read or write
                    var characteristicGuid = characteristic.UUID.ToString();
                    if (OWBoard.SerialWriteUUID.Equals(characteristicGuid, StringComparison.InvariantCultureIgnoreCase) == false &&
                        OWBoard.SerialReadUUID.Equals(characteristicGuid, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        // If our system is little endian, reverse the array.
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(dataBytes);
                        }
                    }
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

            var writeCharacteristicValueRequest = _writeQueue.FirstOrDefault(t => t.CharacteristicId.Equals(characteristic.UUID));
            if (writeCharacteristicValueRequest == null)
            {
                return;
            }

            var data = characteristic.Value;
            byte[] dataBytes;
            if (data != null)
            {
                dataBytes = new byte[data.Length];
                System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));

                var characteristicGuid = characteristic.UUID.ToString();
                if (OWBoard.SerialWriteUUID.Equals(characteristicGuid, StringComparison.InvariantCultureIgnoreCase) == false &&
                    OWBoard.SerialReadUUID.Equals(characteristicGuid, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    // If our system is little endian, reverse the array.
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(dataBytes);
                    }
                }
            }
            else
            {
                dataBytes = null;
            }

            _writeQueue.Remove(writeCharacteristicValueRequest);
            writeCharacteristicValueRequest.CompletionSource.SetResult(dataBytes);
        }
        #endregion

        #region ICBCentralManagerDelegate
        [Export("centralManager:didDiscoverPeripheral:advertisementData:RSSI:")]
        public void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            Debug.WriteLine("CentralManager_DiscoveredPeripheral: " + peripheral.Name);

            var board = new OWBaseBoard()
            {
                ID = peripheral.Identifier.ToString(),
                Name = peripheral.Name ?? "Onewheel",
                IsAvailable = true,
                NativePeripheral = peripheral,
            };

            BoardDiscovered?.Invoke(board);
        }

        [Export("centralManager:didConnectPeripheral:")]
        public void CentralManager_ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            Debug.WriteLine("CentralManager_ConnectedPeripheral: " + peripheral.Name);

            if (_reconnecting)
            {
                _reconnecting = false;
                BoardReconnected?.Invoke();
            }
            else
            {
                //_connectionCompletionSource.SetResult(true);

                var services = _peripheral.Services;
                _peripheral.DiscoverServices(new CBUUID[] { OWBoard.ServiceUUID.ToCBUUID() });
            }


        }

        [Export("centralManager:didDisconnectPeripheral:error:")]
        public void CentralManager_DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Debug.WriteLine("CentralManager_DisconnectedPeripheral");

            BoardDisconnected?.Invoke();


            if (_requestingDisconnect)
            {
                // Disconnect was because the user hit the disconnect button.
                if (_disconnectionCompletionSource != null)
                {
                    _disconnectionCompletionSource.TrySetResult(true);
                }
            }
            else
            {
                Reconnect();
            }
        }

        [Export("centralManager:didFailToConnectPeripheral:error:")]
        public void CentralManager_FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Debug.WriteLine("CentralManager_FailedToConnectPeripheral");
            if (_reconnecting)
            {
                Reconnect();
            }
            else
            {
                ErrorOccurred?.Invoke("Unable to connect to board.");
            }

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
            Debug.WriteLine("CBCentralManager_UpdatedState: " + central.State);
            if (central.State == CBCentralManagerState.PoweredOn)
            {
                // Bluetooth chip is authorised and turned on, start scan.
                DoActualScan();
            }
            else if (central.State == CBCentralManagerState.PoweredOff)
            {
                // User has turned off bluetooth.
                StopScanning();
                ErrorOccurred?.Invoke("Unable to scan for boards, bluetooth is currently disabled.");
            }
            else if (_centralManager.State == CBCentralManagerState.Unauthorized) // User has rejected authorisation.
            {
                ErrorOccurred?.Invoke("Unable to scan for boards without granting bluetooth permissions.");
            }
            else if (_centralManager.State == CBCentralManagerState.Resetting)
            {
                ErrorOccurred?.Invoke("Could not scan for boards at this time.");
            }
            else if (_centralManager.State == CBCentralManagerState.Unsupported)
            {
                ErrorOccurred?.Invoke("Bluetooth scanning is not supported on this device.");
            }
        }

        private void CentralManager_WillRestoreState(object sender, CBWillRestoreEventArgs e)
        {
            Debug.WriteLine("CentralManager_WillRestoreState");

        }
        #endregion

      
        #region IOWBLE
   
        public Task<bool> Connect(OWBaseBoard board, CancellationToken cancellationToken)
        {
            _requestingDisconnect = false;
            _reconnecting = false;

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
                _connectionCompletionSource.SetResult(false);
            }

            return _connectionCompletionSource.Task;
        }

        void Reconnect()
        {
            // Disconnect was because board lost connection.
            BoardReconnecting?.Invoke();
            _reconnecting = true;

            var options = new PeripheralConnectionOptions()
            {
                NotifyOnDisconnection = true,
#if __IOS__
                NotifyOnConnection = true,
                NotifyOnNotification = true,
#endif
            };

            _centralManager.ConnectPeripheral(_peripheral, options);
        }


        public Task Disconnect()
        {
            Debug.WriteLine("Disconnect");
            _requestingDisconnect = true;
            _disconnectionCompletionSource = new TaskCompletionSource<bool>();
            _centralManager.CancelPeripheralConnection(_peripheral);
            return _disconnectionCompletionSource.Task;
        }

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

        public Task<byte[]> WriteValue(string characteristicGuid, byte[] data, bool overrideExistingQueue = false)
        {
            var cbuuid = CBUUID.FromString(characteristicGuid);

            // TODO: Check for connected devices?
            if (_characteristics.ContainsKey(cbuuid) == false)
            {
                // TODO Error?
                return null;
            }

            byte[] dataCopy = null;
            if (data != null)
            {
                dataCopy = new byte[data.Length];
                Array.Copy(data, dataCopy, data.Length);
            }

            if (OWBoard.SerialWriteUUID.Equals(characteristicGuid, StringComparison.InvariantCultureIgnoreCase) == false &&
                OWBoard.SerialReadUUID.Equals(characteristicGuid, StringComparison.InvariantCultureIgnoreCase) == false)
            {
                // If our system is little endian, reverse the array.
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(dataCopy);
                }
            }

            var characteristic = _characteristics[cbuuid];
            var nsData = NSData.FromArray(dataCopy);

            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            CharacteristicValueRequest characteristicValueRequest = new CharacteristicValueRequest(cbuuid, taskCompletionSource, nsData);


            if (overrideExistingQueue)
            {
                _writeQueue.RemoveAll(t => t.CharacteristicId.Equals(cbuuid));
            }
            _writeQueue.Add(characteristicValueRequest);

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
        #endregion
    }
}
