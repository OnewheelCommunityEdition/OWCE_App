using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
//using Android.OS;
using Android.Runtime;
using Java.Util;
using OWCE.DependencyInterfaces;
using OWCE.Droid.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.Droid.DependencyImplementations.OWBLE))]

namespace OWCE.Droid.DependencyImplementations
{
    public class OWBLE : Java.Lang.Object, IOWBLE, INotifyPropertyChanged
    {
        private enum OWBLE_QueueItemOperationType
        {
            Read,
            Write,
            Subscribe,
            Unsubscribe,
        }

        private Queue<OWBLE_QueueItem> _gattOperationQueue = new Queue<OWBLE_QueueItem>();
        private bool _gattOperationQueueProcessing = false;

        public event PropertyChangedEventHandler PropertyChanged;


        private class OWBLE_QueueItem
        {
            public OWBLE_QueueItemOperationType OperationType { get; private set; }
            public BluetoothGattCharacteristic Characteristic { get; private set; }
            public byte[] Data { get; set; }

            public OWBLE_QueueItem(BluetoothGattCharacteristic characteristic, OWBLE_QueueItemOperationType operationType, byte[] data = null)
            {
                Characteristic = characteristic;
                OperationType = operationType;
                Data = data; 
            }
        }

        private class OWBLE_ScanCallback : ScanCallback
        {
            private OWBLE _owble;

            public OWBLE_ScanCallback(OWBLE owble)
            {
                _owble = owble;
            }

            public override void OnBatchScanResults(IList<ScanResult> results)
            {
                Debug.WriteLine("OnBatchScanResults");
                base.OnBatchScanResults(results);
            }

            public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
            {
                Debug.WriteLine("OnScanResult");
                
                var board = new OWBaseBoard()
                {
                    ID = result.Device.Address,
                    Name = result.Device.Name ?? "Onewheel",
                    IsAvailable = true,
                    NativePeripheral = result.Device,
                };

                _owble.BoardDiscovered?.Invoke(board);
            }

            public override void OnScanFailed([GeneratedEnum] ScanFailure errorCode)
            {
                Debug.WriteLine("OnScanFailed");
                base.OnScanFailed(errorCode);
            }
        }
        
        private class OWBLE_LeScanCallback : Java.Lang.Object, BluetoothAdapter.ILeScanCallback
        {
            private OWBLE _owble;

            public OWBLE_LeScanCallback(OWBLE owble)
            {
                _owble = owble;
            }

            public void OnLeScan(BluetoothDevice device, int rssi, byte[] scanRecord)
            {
                Debug.WriteLine("OnLeScan");
                
                var board = new OWBaseBoard()
                {
                    ID = device.JniIdentityHashCode.ToString(),
                    Name = device.Name ?? "Onewheel",
                    IsAvailable = true,
                    NativePeripheral = device,
                };

                _owble.BoardDiscovered?.Invoke(board);
            }
        }

        private class OWBLE_BluetoothGattCallback : BluetoothGattCallback
        {
            private OWBLE _owble;

            public OWBLE_BluetoothGattCallback(OWBLE owble)
            {
                _owble = owble;
            }
                        
            public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
            {
                Debug.WriteLine("OnServicesDiscovered: " + status);
                _owble.OnServicesDiscovered(gatt, status);
            }

            public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
            {
                Debug.WriteLine("OnConnectionStateChange: " + status);
                _owble.OnConnectionStateChange(gatt, status, newState);
            }

            public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
            {
                Debug.WriteLine("OnCharacteristicRead: " + characteristic.Uuid);
                _owble.OnCharacteristicRead(gatt, characteristic, status);
            }

            public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
            {
                Debug.WriteLine("OnCharacteristicWrite: " + characteristic.Uuid);
                _owble.OnCharacteristicWrite(gatt, characteristic, status);
            }

            public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
            {
                Debug.WriteLine("OnCharacteristicChanged: " + characteristic.Uuid);
                _owble.OnCharacteristicChanged(gatt, characteristic);
            }

            public override void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
            {
                Debug.WriteLine($"OnDescriptorRead: {descriptor.Characteristic.Uuid}, {descriptor.Uuid}");
                _owble.OnDescriptorRead(gatt, descriptor, status);
            }

            public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
            {
                Debug.WriteLine($"OnDescriptorWrite: {descriptor.Characteristic.Uuid}, {descriptor.Uuid}");
                _owble.OnDescriptorWrite(gatt, descriptor, status);
            }

            public override void OnReadRemoteRssi(BluetoothGatt gatt, int rssi, [GeneratedEnum] GattStatus status)
            {
                Debug.WriteLine($"OnReadRemoteRssi: {rssi}");
                _owble.OnReadRemoteRssi(gatt, rssi, status);
            }
        }


        Dictionary<string, BluetoothGattCharacteristic> _characteristics = new Dictionary<string, BluetoothGattCharacteristic>();
        Dictionary<string, TaskCompletionSource<byte[]>> _readQueue = new Dictionary<string, TaskCompletionSource<byte[]>>();
        List<CharacteristicValueRequest> _writeQueue = new List<CharacteristicValueRequest>();
        Dictionary<string, TaskCompletionSource<byte[]>> _subscribeQueue = new Dictionary<string, TaskCompletionSource<byte[]>>();
        Dictionary<string, TaskCompletionSource<byte[]>> _unsubscribeQueue = new Dictionary<string, TaskCompletionSource<byte[]>>();
        List<string> _notifyList = new List<string>();

        private void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            //BTA_GATTC_CONN_MAX
            //BTA_GATTC_NOTIF_REG_MAX

            var service = gatt.GetService(OWBoard.ServiceUUID.ToUUID());

            if (service == null)
                return;
            
            foreach (var characteristic in service.Characteristics)
            {
                _characteristics[characteristic.Uuid.ToString().ToLower()] = characteristic;
            }

            if (_connectTaskCompletionSource?.Task.IsCanceled == false && _connectTaskCompletionSource?.Task.IsCompleted == false)
            {
                _connectTaskCompletionSource.SetResult(true);

                // TODO: Fix this.
                //BoardConnected?.Invoke(new OWBoard(_board));
            }
        }

        /*
        private class OWBLE_BroadcastReceiver : BroadcastReceiver
        {
            private OWBLE _owble;

            public OWBLE_BroadcastReceiver(OWBLE owble)
            {
                _owble = owble;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                Debug.WriteLine("OnReceive: " + intent.Action);

                if (BluetoothAdapter.ActionStateChanged.Equals(intent.Action))
                {
                    var stateInt = intent.GetIntExtra(BluetoothAdapter.ExtraState, -1);

                    Debug.WriteLine("stateInt: " + stateInt);
                    if (stateInt == -1)
                    {
                        return;
                    }

                    var state = (State)stateInt;
                    var bluetoothState = BluetoothState.Unknown;

                    switch (state)
                    {
                        case State.Connected:
                            bluetoothState = BluetoothState.Connected;
                            break;
                        case State.Connecting:
                            bluetoothState = BluetoothState.Connecting;
                            break;
                        case State.Disconnected:
                            bluetoothState = BluetoothState.Disconnected;
                            break;
                        case State.Disconnecting:
                            bluetoothState = BluetoothState.Disconnecting;
                            break;
                        case State.Off:
                            bluetoothState = BluetoothState.Off;
                            break;
                        case State.On:
                            bluetoothState = BluetoothState.On;
                            break;
                        case State.TurningOff:
                            bluetoothState = BluetoothState.TurningOff;
                            break;
                        case State.TurningOn:
                            bluetoothState = BluetoothState.TurningOn;
                            break;
                    }

                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _owble?.BLEStateChanged?.Invoke(bluetoothState);
                    });
                }
            }
        }
        */


        private BluetoothAdapter _adapter;
        private BluetoothLeScanner _bleScanner;
        bool _updatingRSSI = false;


        TaskCompletionSource<bool> _connectTaskCompletionSource = null;
        private OWBaseBoard _board = null;

        //private OWBLE_BroadcastReceiver _broadcastReceiver;
        private OWBLE_ScanCallback _scanCallback;
        private OWBLE_LeScanCallback _leScanCallback;
        private OWBLE_BluetoothGattCallback _gattCallback;
        private BluetoothGatt _bluetoothGatt;

        // Moved to be its own property for debugging.
        private Android.OS.BuildVersionCodes _sdkInt = Android.OS.Build.VERSION.SdkInt;

        public OWBLE()
        {
            //_sdkInt = BuildVersionCodes.JellyBeanMr1;

            /*
            _broadcastReceiver = new OWBLE_BroadcastReceiver(this);
            IntentFilter filter = new IntentFilter(BluetoothAdapter.ActionStateChanged);
            Xamarin.Essentials.Platform.AppContext.RegisterReceiver(_broadcastReceiver, filter);
            */
            BluetoothManager manager = Xamarin.Essentials.Platform.CurrentActivity.GetSystemService(Context.BluetoothService) as BluetoothManager;
            _adapter = manager.Adapter;
        }

        public bool IsEnabled()
        {
            return !(_adapter == null || !_adapter.IsEnabled);
        }

        public void RequestPermission()
        {
            // TODO: Request location.

            // Ensures Bluetooth is available on the device and it is enabled. If not,
            // displays a dialog requesting user permission to enable Bluetooth.
            if (_adapter == null || !_adapter.IsEnabled)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                Xamarin.Essentials.Platform.CurrentActivity.StartActivityForResult(enableBtIntent, MainActivity.REQUEST_ENABLE_BT);
            }
        }
        


        private void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            if (_connectTaskCompletionSource.Task.IsCanceled)
                return;

            if (status == GattStatus.Success)
            {
                gatt.DiscoverServices();
            }
            else
            {
                _connectTaskCompletionSource.SetResult(false);
            }
        }

        private int _queueNumber = 0;

        private void ProcessQueue()
        {
            var queueNumber = _queueNumber;
            ++_queueNumber;

            Debug.WriteLine($"ProcessQueue {queueNumber}: {_gattOperationQueue.Count}");
            if (_gattOperationQueue.Count == 0)
            {
                return;
            }

            if (_gattOperationQueueProcessing)
                return;

            _gattOperationQueueProcessing = true;

            var item = _gattOperationQueue.Dequeue();
            switch (item.OperationType)
            {
                case OWBLE_QueueItemOperationType.Read:
                    bool didRead = _bluetoothGatt.ReadCharacteristic(item.Characteristic);
                    if (didRead == false)
                    {
                        Debug.WriteLine($"ERROR {queueNumber}: Unable to read {item.Characteristic.Uuid}");
                    }
                    break;

                case OWBLE_QueueItemOperationType.Write:
                    bool didSetValue = item.Characteristic.SetValue(item.Data);
                    bool didWrite = _bluetoothGatt.WriteCharacteristic(item.Characteristic);
                    if (didWrite == false)
                    {
                        Debug.WriteLine($"ERROR {queueNumber}: Unable to write {item.Characteristic.Uuid}");
                    }
                    break;

                case OWBLE_QueueItemOperationType.Subscribe:
                    bool didSubscribe = _bluetoothGatt.SetCharacteristicNotification(item.Characteristic, true);
                    if (didSubscribe == false)
                    {
                        Debug.WriteLine($"ERROR {queueNumber}: Unable to subscribe {item.Characteristic.Uuid}");
                    }

                    var subscribeDescriptor = item.Characteristic.GetDescriptor(UUID.FromString("00002902-0000-1000-8000-00805f9b34fb"));
                    bool didSetSubscribeDescriptor = subscribeDescriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray());
                    bool didWriteSubscribeDescriptor = _bluetoothGatt.WriteDescriptor(subscribeDescriptor);
                    break;

                case OWBLE_QueueItemOperationType.Unsubscribe:
                    bool didUnsubscribe = _bluetoothGatt.SetCharacteristicNotification(item.Characteristic, false);
                    if (didUnsubscribe == false)
                    {
                        Debug.WriteLine($"ERROR {queueNumber}: Unable to unsubscribe {item.Characteristic.Uuid}");
                    }

                    var unsubscribeDescriptor = item.Characteristic.GetDescriptor(UUID.FromString("00002902-0000-1000-8000-00805f9b34fb"));
                    var didSetUnsubscribeDescriptor = unsubscribeDescriptor.SetValue(BluetoothGattDescriptor.DisableNotificationValue.ToArray());
                    var didWriteUnsubscribeDescriptor = _bluetoothGatt.WriteDescriptor(unsubscribeDescriptor);
                    break;
            }
        }

        private void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            var uuid = characteristic.Uuid.ToString().ToLower();

            if (_readQueue.ContainsKey(uuid))
            {
                var readItem = _readQueue[uuid];
                _readQueue.Remove(uuid);

                var dataBytes = characteristic.GetValue();


                if (OWBoard.SerialWriteUUID.Equals(uuid, StringComparison.InvariantCultureIgnoreCase) == false &&
                    OWBoard.SerialReadUUID.Equals(uuid, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    // If our system is little endian, reverse the array.
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(dataBytes);
                    }
                }

                readItem.SetResult(dataBytes);
            }

            _gattOperationQueueProcessing = false;
            ProcessQueue();
        }


        private void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            var uuid = characteristic.Uuid.ToString().ToLower();

            var writeCharacteristicValueRequest = _writeQueue.FirstOrDefault(t => t.CharacteristicId.Equals(uuid));


            if (writeCharacteristicValueRequest != null)
            {
                _writeQueue.Remove(writeCharacteristicValueRequest);

                var dataBytes = characteristic.GetValue();

                if (OWBoard.SerialWriteUUID.Equals(uuid, StringComparison.InvariantCultureIgnoreCase) == false &&
                    OWBoard.SerialReadUUID.Equals(uuid, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    // If our system is little endian, reverse the array.
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(dataBytes);
                    }
                }

                writeCharacteristicValueRequest.CompletionSource.SetResult(dataBytes);
            }

            _gattOperationQueueProcessing = false;
            ProcessQueue();
        }


        private void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            var uuid = characteristic.Uuid.ToString().ToLower();

            if (_notifyList.Contains(uuid))
            {
                var dataBytes = characteristic.GetValue();

                if (OWBoard.SerialWriteUUID.Equals(uuid, StringComparison.InvariantCultureIgnoreCase) == false &&
                   OWBoard.SerialReadUUID.Equals(uuid, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    // If our system is little endian, reverse the array.
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(dataBytes);
                    }
                }

                BoardValueChanged.Invoke(uuid, dataBytes);
            }
        }


        public void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, [GeneratedEnum] GattStatus status)
        {
            // TODO: ?
        }

        public void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, [GeneratedEnum] GattStatus status)
        {
            var uuid = descriptor.Characteristic.Uuid.ToString().ToLower();

            // Check if its a subscribe or unsubscribe descriptor
            if (descriptor.Uuid.ToString().ToLower() == "00002902-0000-1000-8000-00805f9b34fb")
            {
                var descriptorValue = descriptor.GetValue();

                if (descriptorValue.SequenceEqual(BluetoothGattDescriptor.EnableNotificationValue.ToArray()))
                {
                    if (_subscribeQueue.ContainsKey(uuid))
                    {
                        var subscribeItem = _subscribeQueue[uuid];
                        _subscribeQueue.Remove(uuid);
                        subscribeItem.SetResult(descriptorValue);
                    }
                }
                else if(descriptorValue.SequenceEqual(BluetoothGattDescriptor.DisableNotificationValue.ToArray()))
                {
                    if (_unsubscribeQueue.ContainsKey(uuid))
                    {
                        var unsubscribeItem = _unsubscribeQueue[uuid];
                        _unsubscribeQueue.Remove(uuid);
                        unsubscribeItem.SetResult(descriptorValue);
                    }
                }
                else
                {
                    Debug.WriteLine($"OnDescriptorWrite Error: Unhandled descriptor of {descriptor.Uuid} on {uuid}.");
                }
            }
            else
            {
                Debug.WriteLine($"OnDescriptorWrite Error: Unhandled descriptor of {descriptor.Uuid} on {uuid}.");
            }

            _gattOperationQueueProcessing = false;
            ProcessQueue();
        }

        public void OnReadRemoteRssi(BluetoothGatt gatt, int rssi, [GeneratedEnum] GattStatus status)
        {
            RSSIUpdated?.Invoke(rssi);
            _updatingRSSI = false;
        }



        #region IOWBLE
        public Action<BluetoothState> BLEStateChanged { get; set; }
        public Action<OWBaseBoard> BoardDiscovered { get; set; }
        public Action<OWBoard> BoardConnected { get; set; }
        public Action<string, byte[]> BoardValueChanged { get; set; }
        public Action<int> RSSIUpdated { get; set; }
        public Action BoardDisconnected { get; set; }
        public Action BoardReconnecting { get; set; }
        public Action BoardReconnected { get; set; }
        public Action<String> ErrorOccurred { get; set; }


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


       
        public Task<bool> Connect(OWBaseBoard board, CancellationToken cancellationToken)
        {
            _board = board;

            _connectTaskCompletionSource = new TaskCompletionSource<bool>();

            if (board.NativePeripheral is BluetoothDevice device)
            {
                _gattCallback = new OWBLE_BluetoothGattCallback(this);
                _bluetoothGatt = device.ConnectGatt(Xamarin.Essentials.Platform.CurrentActivity, false, _gattCallback);
            }

            return _connectTaskCompletionSource.Task;
        }

        public Task Disconnect()
        {
            if (_connectTaskCompletionSource != null && _connectTaskCompletionSource.Task.IsCanceled == false && _connectTaskCompletionSource.Task.IsCompleted == false && _connectTaskCompletionSource.Task.IsFaulted == false)
            {
                try
                {
                    _connectTaskCompletionSource.SetCanceled();
                    _connectTaskCompletionSource = null;

                    _bluetoothGatt.Disconnect();
                    _bluetoothGatt = null;
                    _gattCallback = null;


                    _readQueue.Clear();
                    _writeQueue.Clear();
                    _subscribeQueue.Clear();
                    _unsubscribeQueue.Clear();
                    _gattOperationQueue.Clear();

                }
                catch (Exception err)
                {

                    Debugger.Break();
                }
            }

            // TODO: Handle is connecting.
            if (_bluetoothGatt != null)
            {
                _bluetoothGatt.Disconnect();
            }

            _board = null;

            return Task.CompletedTask;
        }

        public async void StartScanning()
        {
            if (IsScanning)
                return;

            IsScanning = true;

            // TODO: Handle power on state.

            if (_sdkInt >= Android.OS.BuildVersionCodes.Lollipop) // 21
            {
                _bleScanner = _adapter.BluetoothLeScanner;
                _scanCallback = new OWBLE_ScanCallback(this);
                var scanFilters = new List<ScanFilter>();
                var scanSettingsBuilder = new ScanSettings.Builder();

                var scanFilterBuilder = new ScanFilter.Builder();
                scanFilterBuilder.SetServiceUuid(OWBoard.ServiceUUID.ToParcelUuid());
                scanFilters.Add(scanFilterBuilder.Build());
                _bleScanner.StartScan(scanFilters, scanSettingsBuilder.Build(), _scanCallback);
            }
            else if (_sdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr2) // 18
            {
                _leScanCallback = new OWBLE_LeScanCallback(this);
#pragma warning disable 0618
                _adapter.StartLeScan(new Java.Util.UUID[] { OWBoard.ServiceUUID.ToUUID() }, _leScanCallback);
#pragma warning restore 0618
            }
            else
            {
                throw new NotImplementedException("Can't run bluetooth scans on device lower than Android 4.3");
            }

            await Task.Delay(15 * 1000);

            StopScanning();
        }

        public void StopScanning()
        {
            if (IsScanning == false)
                return;


            if (_sdkInt >= Android.OS.BuildVersionCodes.Lollipop) // 21
            {
                _bleScanner.StopScan(_scanCallback);
            }
            else
            {
#pragma warning disable 0618
                _adapter.StopLeScan(_leScanCallback);
#pragma warning restore 0618
            }

            IsScanning = false;
        }


        public Task<byte[]> ReadValue(string characteristicGuid, bool important = false)
        {
            Debug.WriteLine($"ReadValue: {characteristicGuid}");

            if (_bluetoothGatt == null)
                return null;

            var uuid = characteristicGuid.ToLower();

            // TODO: Check for connected devices?
            if (_characteristics.ContainsKey(uuid) == false)
            {
                // TODO Error?
                return null;
            }

            // Already awaiting it.
            if (_readQueue.ContainsKey(uuid))
            {
                return _readQueue[uuid].Task;
            }

            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            if (important)
            {
                // TODO: Put this at the start of the queue.
                _readQueue.Add(uuid, taskCompletionSource);
            }
            else
            {
                _readQueue.Add(uuid, taskCompletionSource);
            }

            _gattOperationQueue.Enqueue(new OWBLE_QueueItem(_characteristics[uuid], OWBLE_QueueItemOperationType.Read));

            ProcessQueue();

            return taskCompletionSource.Task;
        }

        public Task<byte[]> WriteValue(string characteristicGuid, byte[] data, bool overrideExistingQueue = false)
        {
            Debug.WriteLine($"WriteValue: {characteristicGuid}");
            if (_bluetoothGatt == null)
                return null;

            if (data.Length > 20)
            {
                // TODO: Error, some Android BLE devices do not handle > 20byte packets well.
                return null;
            }

            var uuid = characteristicGuid.ToLower();

            // TODO: Check for connected devices?
            if (_characteristics.ContainsKey(uuid) == false)
            {
                // TODO Error?
                return null;
            }

            // TODO: Handle this.
            /*
            if (_readQueue.ContainsKey(uuid))
            {
                return _readQueue[uuid].Task;
            }
            */

            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            CharacteristicValueRequest characteristicValueRequest = new CharacteristicValueRequest(uuid, taskCompletionSource, data);


            if (overrideExistingQueue)
            {
                _writeQueue.RemoveAll(t => t.CharacteristicId.Equals(uuid));
            }
            _writeQueue.Add(characteristicValueRequest);

           
            byte[] dataBytes = null;
            if (data != null)
            {
                dataBytes = new byte[data.Length];
                Array.Copy(data, dataBytes, data.Length);
            
                if (OWBoard.SerialWriteUUID.Equals(uuid, StringComparison.InvariantCultureIgnoreCase) == false &&
                       OWBoard.SerialReadUUID.Equals(uuid, StringComparison.InvariantCultureIgnoreCase) == false)
                {
                    // If our system is little endian, reverse the array.
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(dataBytes);
                    }
                }
            }


            _gattOperationQueue.Enqueue(new OWBLE_QueueItem(_characteristics[uuid], OWBLE_QueueItemOperationType.Write, dataBytes));

            ProcessQueue();

            return taskCompletionSource.Task;
        }

        public Task SubscribeValue(string characteristicGuid, bool important = false)
        {
            Debug.WriteLine($"SubscribeValue: {characteristicGuid}");
            if (_bluetoothGatt == null)
                return null;

            var uuid = characteristicGuid.ToLower();

            // TODO: Check for connected devices?
            if (_characteristics.ContainsKey(uuid) == false)
            {
                // TODO Error?
                return null;
            }

            _notifyList.Add(uuid);

            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            if (important)
            {
                // TODO: Put this at the start of the queue.
                _subscribeQueue.Add(uuid, taskCompletionSource);
            }
            else
            {
                _subscribeQueue.Add(uuid, taskCompletionSource);
            }

            _gattOperationQueue.Enqueue(new OWBLE_QueueItem(_characteristics[uuid], OWBLE_QueueItemOperationType.Subscribe));

            ProcessQueue();

            return taskCompletionSource.Task;
        }

        public Task UnsubscribeValue(string characteristicGuid, bool important = false)
        {
            Debug.WriteLine($"UnsubscribeValue: {characteristicGuid}");
            if (_bluetoothGatt == null)
                return null;

            var uuid = characteristicGuid.ToLower();

            // TODO: Check for connected devices?
            if (_characteristics.ContainsKey(uuid) == false)
            {
                // TODO Error?
                return null;
            }

            _notifyList.RemoveAll(x => x == uuid);

            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            if (important)
            {
                // TODO: Put this at the start of the queue.
                _unsubscribeQueue.Add(uuid, taskCompletionSource);
            }
            else
            {
                _unsubscribeQueue.Add(uuid, taskCompletionSource);
            }

            _gattOperationQueue.Enqueue(new OWBLE_QueueItem(_characteristics[uuid], OWBLE_QueueItemOperationType.Unsubscribe));

            ProcessQueue();

            return taskCompletionSource.Task;
        }

        public bool BluetoothEnabled()
        {
            var bluetoothAdapter = Android.Bluetooth.BluetoothAdapter.DefaultAdapter;
            if (bluetoothAdapter == null)
            {
                // Device does not support Bluetooth
                return false;
            }
            else if (bluetoothAdapter.IsEnabled == false)
            {
                // Bluetooth is not enabled
                return false;
            }

            // Bluetooth is enabled 
            return true;
        }

        public async Task<bool> ReadyToScan()
        {
            if ((int)Android.OS.Build.VERSION.SdkInt >= 31)
            {
                var permissionStatus = await Permissions.CheckStatusAsync<BluetoothPermission>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    return true;
                }
            }
            else
            {
                var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (permissionStatus == PermissionStatus.Granted || permissionStatus == PermissionStatus.Restricted)
                {
                    return true;
                }
            }

            return false;
        }

        public void Shutdown()
        {
            // TODO: Handle this.
        }

        public void RequestRSSIUpdate()
        {
            if (_updatingRSSI)
            {
                return;
            }

            _updatingRSSI = true;
            _bluetoothGatt?.ReadRemoteRssi();
        }
        #endregion
    }
}
