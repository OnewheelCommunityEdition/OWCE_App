using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OWCE.DependencyInterfaces;
using OWCE.Protobuf;

namespace OWCE
{
    public class MockOWBLE : IOWBLE, INotifyPropertyChanged
    {
        const string RSSIKey = "RSSI";
        public event PropertyChangedEventHandler PropertyChanged;

        string _sampleRideName;
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

            OWBoardEvent previousEvent = null;
            OWBoardEvent currentEvent = null;
            try
            {
                var assembly = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;

                var expectedResourceFile = $"OWCE.Resources.SampleRideData.{_sampleRideName}.bin";
                var logFileFound = assembly.GetManifestResourceNames().Contains(expectedResourceFile);
                if (logFileFound == false)
                {
                    return;
                }

                using (var stream = assembly.GetManifestResourceStream(expectedResourceFile))
                {
                    do
                    {
                        previousEvent = currentEvent;
                        currentEvent = OWBoardEvent.Parser.ParseDelimitedFrom(stream);

                        if (previousEvent != null)
                        {
                            long sleepDuration = currentEvent.Timestamp - previousEvent.Timestamp;                           
                            Thread.Sleep((int)sleepDuration);
                        }

                        if (currentEvent.Uuid == RSSIKey)
                        {
                            RSSIUpdated?.Invoke(BitConverter.ToInt32(currentEvent.Data.ToByteArray()));
                        }
                        else
                        {
                            BoardValueChanged.Invoke(currentEvent.Uuid, currentEvent.Data.ToByteArray());
                        }

                    }
                    while (stream.Position < stream.Length);
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
        public Action<int> RSSIUpdated { get; set; }
        public Action BoardDisconnected { get; set; }
        public Action BoardReconnecting { get; set; }
        public Action BoardReconnected { get; set; }


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

            if (board.NativePeripheral is String sampleRideName)
            {
                _sampleRideName = sampleRideName;

                _messagePumpThread = new Thread(MessagePump);
                _messagePumpThread.Start();

                //board.BoardType = OWBoardType.Plus;

                
                return true;
            }

            return false;
        }

        public async Task Disconnect()
        {
            await Task.Delay(1000);
            BoardDisconnected?.Invoke();
        }

        public Task<byte[]> ReadValue(string characteristicGuid, bool important = false)
        {
            return null;
        }

        public void StartScanning()
        {
            IsScanning = true;

            System.Diagnostics.Debug.WriteLine($"Logs directory: {App.Current.LogsDirectory}");


            var rand = new Random();
            var filenameRegex = new System.Text.RegularExpressions.Regex(@"^OWCE\.Resources\.SampleRideData\.(.*)\.bin$");
            var assembly = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                var match = filenameRegex.Match(resourceName);
                if (match.Success)
                {
                    using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {

                        // Can't use real board serial for this as multiple test data of the same board would all appear as the same board,
                        // which means it would not show. Instead we generate a 6 digit number based on the MD5 hash of the file. It isn't
                        // perfect (potential collisions, etc) but it should work fine for what we are doing.
                        //
                        // The reason we want unique rather than random is because when you come back to the board list page you will see
                        // duplicate results as they will have different device IDs. If it's based on something that doesn't change such as
                        // the hash of a file this will prevent duplicates re-appearing.
                        //
                        // While we don't use the hash itself, we use this to seed our random number generator which should give the same
                        // number every time.
                        var fakeDeviceID = String.Empty;
                        using (var md5 = System.Security.Cryptography.MD5.Create())
                        {
                            // Hash will be 16 bytes. Seed is an int which is 4 bytes. So we will instead take the average of every 4
                            // bytes of the hash.
                            var hash = md5.ComputeHash(resourceStream);
                            var shrunkHash = new byte[4];
                            for (int startIndex = 0, outIndex = 0; startIndex < 16; startIndex += 4, outIndex += 1)
                            {
                                var sum = hash[startIndex + 0] + hash[startIndex + 1] + hash[startIndex + 2] + hash[startIndex + 3];
                                shrunkHash[outIndex] = (byte)(sum / 4);
                            }
                            var shrunkHashNumber = BitConverter.ToInt32(shrunkHash);

                            var random = new Random(shrunkHashNumber);
                            fakeDeviceID = random.Next(0, 999999).ToString("D6");
                        }

                        // Fallback incase something bad happened.
                        if (String.IsNullOrEmpty(fakeDeviceID))
                        {
                            fakeDeviceID = rand.Next(0, 999999).ToString("D6");
                        }

                        BoardDiscovered?.Invoke(new OWBaseBoard()
                        {
                            ID = "ow" + fakeDeviceID,
                            Name = match.Groups[1].Value,
                            IsAvailable = true,
                            NativePeripheral = match.Groups[1].Value,
                        });
                    }
                }
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

        public async Task<byte[]> WriteValue(string characteristicGuid, byte[] data, bool overrideExistingQueue = false)
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

        public void RequestRSSIUpdate()
        {

        }


        public Task<bool> ReadyToScan()
        {
            return Task.FromResult<bool>(true);
        }
    }
}
