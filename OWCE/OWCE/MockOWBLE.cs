using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OWCE.DependencyInterfaces;
using OWCE.Protobuf;

namespace OWCE
{
    public class MockOWBLE : IOWBLE, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string _logFilename;
        List<OWBoardEvent> _events = new List<OWBoardEvent>();
        Thread _messagePumpThread;

        public MockOWBLE()
        {

        }

        void MessagePump()
        {
            while (BoardValueChanged == null)
            {
                Thread.Sleep(100);
            }

            ushort firmwareRevision = 4034;
            ushort hardwareRevision = 3206;

            BoardValueChanged.Invoke(OWBoard.FirmwareRevisionUUID, BitConverter.GetBytes(firmwareRevision));
            BoardValueChanged.Invoke(OWBoard.HardwareRevisionUUID, BitConverter.GetBytes(hardwareRevision));

            OWBoardEvent previousEvent = null;
            OWBoardEvent currentEvent = null;
            try
            {
              
                using (FileStream fs = new FileStream(_logFilename, FileMode.Open, FileAccess.Read))
                {
                    do
                    {
                        previousEvent = currentEvent;
                        currentEvent = OWBoardEvent.Parser.ParseDelimitedFrom(fs);

                        if (previousEvent != null)
                        {
                            long sleepDuration = currentEvent.Timestamp - previousEvent.Timestamp;                           
                            Thread.Sleep((int)sleepDuration);
                        }

                        if (currentEvent.Uuid == OWBoard.FirmwareRevisionUUID)
                        {
                            int x = 0;

                            /*
                            if (currentEvent.da. >= 4000)
                            {
                                BatteryCells.CellCount = 15;
                                BatteryCells.IgnoreCell(15);
                                OnPropertyChanged("BatteryCells");
                            }
                            else
                            {
                                BatteryCells.CellCount = 16;
                            }
                            */
                        }
                        BoardValueChanged.Invoke(currentEvent.Uuid, currentEvent.Data.ToByteArray());

                    }
                    while (fs.Position < fs.Length);
                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: " + err.Message);
            }
        }

        public Action<BluetoothState> BLEStateChanged { get; set; }
        public Action<OWBaseBoard> BoardDiscovered { get; set; }
        public Action<OWBoard> BoardConnected { get; set; }
        public Action<string, byte[]> BoardValueChanged { get; set; }
        public Action<string> ErrorOccurred { get; set; }

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


        public bool BluetoothEnabled()
        {
            return true;
        }

        public async Task<bool> Connect(OWBaseBoard board, CancellationToken cancellationToken)
        {
            await Task.Delay(500);

            if (board.NativePeripheral is String logFilename)
            {
                _logFilename = logFilename;

                _messagePumpThread = new Thread(MessagePump);
                _messagePumpThread.Start();

                board.BoardType = OWBoardType.Plus;

                
                return true;
            }

            return false;
        }

        public async Task Disconnect()
        {
            await Task.Delay(1000);
        }

        public Task<byte[]> ReadValue(string characteristicGuid, bool important = false)
        {
            return null;
        }

        public void StartScanning()
        {
            IsScanning = true;

            var files = Directory.GetFiles(App.Current.LogsDirectory, "*.bin");
            var rand = new Random();
            foreach (var file in files)
            {
                BoardDiscovered?.Invoke(new OWBaseBoard()
                {
                    ID = "ow" + rand.Next(0, 999999).ToString("D6"),
                    Name = Path.GetFileNameWithoutExtension(file),
                    IsAvailable = true,
                    NativePeripheral = file,
                });
            }
        }

        public void StopScanning()
        {
            IsScanning = false;
        }

        public async Task SubscribeValue(string characteristicGuid, bool important = false)
        {
            await Task.Delay(10);
        }

        public async Task UnsubscribeValue(string characteristicGuid, bool important = false)
        {
            await Task.Delay(10);
        }

        public async Task<byte[]> WriteValue(string characteristicGuid, byte[] data, bool important = false)
        {
            await Task.Delay(10);
            return null;
        }

        public void Shutdown()
        {
            if (_messagePumpThread != null)
            {
                _messagePumpThread.Abort();
                _messagePumpThread = null;
            }
        }


        public bool ReadyToScan()
        {
            return true;
        }
    }
}
