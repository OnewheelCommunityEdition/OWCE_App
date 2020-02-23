using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using System.Text.Json;
using OWCE.Protobuf;
//using Plugin.Geolocator;
//using Plugin.Geolocator.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using OWCE.Models;

namespace OWCE
{
    public enum OWBoardType
    {
        Unknown,
        V1,
        Plus,
        XR,
        Pint,
    };

    public class OWBoard : OWBaseBoard
    {
        public static readonly Guid ServiceUUID = new Guid("E659F300-EA98-11E3-AC10-0800200C9A66");
        public const string SerialNumberUUID = "E659F301-EA98-11E3-AC10-0800200C9A66";
        public const string RideModeUUID = "E659F302-EA98-11E3-AC10-0800200C9A66";
        public const string BatteryPercentUUID = "E659F303-EA98-11E3-AC10-0800200C9A66";
        public const string BatteryLow5UUID = "E659F304-EA98-11E3-AC10-0800200C9A66";
        public const string BatteryLow20UUID = "E659F305-EA98-11E3-AC10-0800200C9A66";
        public const string BatterySerialUUID = "E659F306-EA98-11E3-AC10-0800200C9A66";
        public const string PitchUUID = "E659F307-EA98-11E3-AC10-0800200C9A66";
        public const string RollUUID = "E659F308-EA98-11E3-AC10-0800200C9A66";
        public const string YawUUID = "E659F309-EA98-11E3-AC10-0800200C9A66";
        public const string TripOdometerUUID = "E659F30A-EA98-11E3-AC10-0800200C9A66";
        public const string RpmUUID = "E659F30B-EA98-11E3-AC10-0800200C9A66";
        public const string LightModeUUID = "E659F30C-EA98-11E3-AC10-0800200C9A66";
        public const string LightsFrontUUID = "E659F30D-EA98-11E3-AC10-0800200C9A66";
        public const string LightsBackUUID = "E659F30E-EA98-11E3-AC10-0800200C9A66";
        public const string StatusErrorUUID = "E659F30F-EA98-11E3-AC10-0800200C9A66";
        public const string TemperatureUUID = "E659F310-EA98-11E3-AC10-0800200C9A66";
        public const string FirmwareRevisionUUID = "E659F311-EA98-11E3-AC10-0800200C9A66";
        public const string CurrentAmpsUUID = "E659F312-EA98-11E3-AC10-0800200C9A66";
        public const string TripAmpHoursUUID = "E659F313-EA98-11E3-AC10-0800200C9A66";
        public const string TripRegenAmpHoursUUID = "E659F314-EA98-11E3-AC10-0800200C9A66";
        public const string BatteryTemperatureUUID = "E659F315-EA98-11E3-AC10-0800200C9A66";
        public const string BatteryVoltageUUID = "E659F316-EA98-11E3-AC10-0800200C9A66";
        public const string SafetyHeadroomUUID = "E659F317-EA98-11E3-AC10-0800200C9A66";
        public const string HardwareRevisionUUID = "E659F318-EA98-11E3-AC10-0800200C9A66";
        public const string LifetimeOdometerUUID = "E659F319-EA98-11E3-AC10-0800200C9A66";
        public const string LifetimeAmpHoursUUID = "E659F31A-EA98-11E3-AC10-0800200C9A66";
        public const string BatteryCellsUUID = "E659F31B-EA98-11E3-AC10-0800200C9A66";
        public const string LastErrorCodeUUID = "E659F31C-EA98-11E3-AC10-0800200C9A66";
        public const string SerialReadUUID = "E659F3FE-EA98-11E3-AC10-0800200C9A66";
        public const string SerialWriteUUID = "E659F3FF-EA98-11E3-AC10-0800200C9A66";
        public const string UNKNOWN1UUID = "E659F31D-EA98-11E3-AC10-0800200C9A66";
        public const string UNKNOWN2UUID = "E659F31E-EA98-11E3-AC10-0800200C9A66";
        public const string UNKNOWN3UUID = "E659F31F-EA98-11E3-AC10-0800200C9A66";
        public const string UNKNOWN4UUID = "E659F320-EA98-11E3-AC10-0800200C9A66";

        protected IntBoardDetail _serialNumber = new IntBoardDetail("Serial number");
        protected IntBoardDetail _batteryPercent = new IntBoardDetail("Battery percent");
        protected IntBoardDetail _batteryLow5 = new IntBoardDetail("Battery low 5");
        protected IntBoardDetail _batteryLow20 = new IntBoardDetail("Battery low 20");
        protected IntBoardDetail _batterySerial = new IntBoardDetail("Battery serial number");
        protected AngleBoardDetail _pitch = new AngleBoardDetail("Pitch");
        protected AngleBoardDetail _yaw = new AngleBoardDetail("Yaw");
        protected AngleBoardDetail _roll = new AngleBoardDetail("Roll");
        protected DistanceBoardDetail _tripOdometer = new DistanceBoardDetail("Trip odometer");
        protected SpeedBoardDetail _speed = new SpeedBoardDetail("Speed");
        protected IntBoardDetail _statusError = new IntBoardDetail("Error status");
        protected TemperatureBoardDetail _controllerTemperature = new TemperatureBoardDetail("Controller temperature");
        protected TemperatureBoardDetail _motorTemperature = new TemperatureBoardDetail("Motor temperature");
        protected IntBoardDetail _firmwareRevision = new IntBoardDetail("Firmware");
        protected AmpBoardDetail _currentAmps = new AmpBoardDetail("Current power draw");
        protected AmpHoursBoardDetail _tripAmpHours = new AmpHoursBoardDetail("Trip usage");
        protected AmpHoursBoardDetail _tripRegenAmpHours = new AmpHoursBoardDetail("Trip regen");
        protected TemperatureBoardDetail _unknownTemperature = new TemperatureBoardDetail("Unknown temperature");
        protected TemperatureBoardDetail _batteryTemperature = new TemperatureBoardDetail("Battery temperature");
        protected VoltageBoardDetail _batteryVoltage = new VoltageBoardDetail("Battery voltage");
        protected IntBoardDetail _safetyHeadroom = new IntBoardDetail("Safety headroom");
        protected DistanceBoardDetail _lifetimeOdometer = new DistanceBoardDetail("Lifetime odometer");
        protected AmpHoursBoardDetail _lifetimeAmpHours = new AmpHoursBoardDetail("Lifetime amp hours");
        protected FloatBoardDetail _lastErrorCode = new FloatBoardDetail("Last error code");
        protected BatteryCellsBoardDetail _batteryCells = new BatteryCellsBoardDetail("Battery cells");
        protected IntBoardDetail _rpm = new IntBoardDetail("Rotations per minute");
        protected IntBoardDetail _hardwareRevision = new IntBoardDetail("Hardware revision");
        protected RideModeBoardDetail _rideMode = new RideModeBoardDetail("Ride mode");





        private OWBoardType _boardType = OWBoardType.Unknown;
        public OWBoardType BoardType
        {
            get
            {
                return _boardType;
            }
            set
            {
                if (_boardType != value)
                {
                    _boardType = value;
                    _rideMode.BoardType = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BoardModelString
        {
            get
            {
                return BoardType switch
                {
                    OWBoardType.V1 => "V1",
                    OWBoardType.Plus => "Plus",
                    OWBoardType.XR => "XR",
                    OWBoardType.Pint => "Pint",
                    _ => String.Empty,
                };
            }
        }

        public int MaxRecommendedSpeed
        {
            get
            {
                if (App.Current.MetricDisplay)
                {
                    if (_boardType == OWBoardType.V1)
                    {
                        return _rideMode.Value switch
                        {
                            1 => 19, // Classic
                            2 => 24, // Extreme
                            3 => 24, // Elevated
                            _ => 24, // Unknown
                        };
                    }
                    else if (_boardType == OWBoardType.Plus || _boardType == OWBoardType.XR)
                    {
                        return _rideMode.Value switch
                        {
                            4 => 19, // Sequoia
                            5 => 24, // Cruz
                            6 => 30, // Mission
                            7 => 30, // Elevated
                            8 => 32, // Delirium
                            9 => 32, // Custom
                            _ => 32, // Unknown
                        };
                    }
                    else if (_boardType == OWBoardType.Pint)
                    {
                        return _rideMode.Value switch
                        {
                            5 => 19, // Redwood
                            6 => 26, // Pacific
                            7 => 26, // Elevated
                            8 => 26, // Skyline
                            _ => 26, // Unknown
                        };
                    }

                    return 32;
                }
                else
                {
                    if (_boardType == OWBoardType.V1)
                    {
                        return _rideMode.Value switch
                        {
                            1 => 12, // Classic
                            2 => 15, // Extreme
                            3 => 15, // Elevated
                            _ => 15, // Unknown
                        };
                    }
                    else if (_boardType == OWBoardType.Plus || _boardType == OWBoardType.XR)
                    {
                        return _rideMode.Value switch
                        {
                            4 => 12, // Sequoia
                            5 => 15, // Cruz
                            6 => 19, // Mission
                            7 => 19, // Elevated
                            8 => 20, // Delirium
                            9 => 20, // Custom
                            _ => 20, // Unknown
                        };
                    }
                    else if (_boardType == OWBoardType.Pint)
                    {
                        return _rideMode.Value switch
                        {
                            5 => 12, // Redwood
                            6 => 16, // Pacific
                            7 => 16, // Elevated
                            8 => 16, // Skyline
                            _ => 16, // Unknown
                        };
                    }

                    // New ride mode?
                    return 20;
                }
            }
        }

        public int MaxDisplaySpeed
        {
            get
            {
                if (App.Current.SpeedDemon == false)
                {
                    return MaxRecommendedSpeed;
                }

                if (App.Current.MetricDisplay)
                {
                    return 50;
                }
                else
                {
                    return 30;
                }
            }
        }

        public SpeedBoardDetail Speed
        {
            get { return _speed; }
        }

        


        




        private bool _lightMode = false;
        public bool LightMode
        {
            get { return _lightMode; }
            set { if (_lightMode != value) { _lightMode = value; OnPropertyChanged(); } }
        }

        private int _frontLightMode = 0;
        public int FrontLightMode
        {
            get { return _frontLightMode; }
            set { if (_frontLightMode != value) { _frontLightMode = value; OnPropertyChanged(); } }
        }

        private int _rearLightMode = 0;
        public int RearLightMode
        {
            get { return _rearLightMode; }
            set { if (_rearLightMode != value) { _rearLightMode = value; OnPropertyChanged(); } }
        }






        private float _UNKNOWN1 = 0;
        public float UNKNOWN1
        {
            get { return _UNKNOWN1; }
            set { if (_UNKNOWN1.AlmostEqualTo(value) == false) { _UNKNOWN1 = value; OnPropertyChanged(); } }
        }

        private float _UNKNOWN2 = 0;
        public float UNKNOWN2
        {
            get { return _UNKNOWN2; }
            set { if (_UNKNOWN2.AlmostEqualTo(value) == false) { _UNKNOWN2 = value; OnPropertyChanged(); } }
        }

        private float _UNKNOWN3 = 0;
        public float UNKNOWN3
        {
            get { return _UNKNOWN3; }
            set { if (_UNKNOWN3.AlmostEqualTo(value) == false) { _UNKNOWN3 = value; OnPropertyChanged(); } }
        }

        private float _UNKNOWN4 = 0;
        public float UNKNOWN4
        {
            get { return _UNKNOWN4; }
            set { if (_UNKNOWN4.AlmostEqualTo(value) == false) { _UNKNOWN4 = value; OnPropertyChanged(); } }
        }

        private int _rssi = 0;
        public int RSSI
        {
            get { return _rssi; }
            set { if (_rssi != value) { _rssi = value; OnPropertyChanged(); } }
        }


        private OWBoardEventList _events = new OWBoardEventList();
        private List<OWBoardEvent> _initialEvents = new List<OWBoardEvent>();
        private Ride _currentRide = null;
        private bool _keepHandshakeBackgroundRunning = false;

        public List<BaseBoardDetail> FullBoardDetailsList { get; } = new List<BaseBoardDetail>();
        public ObservableCollection<BaseBoardDetail> SelectedBoardDetailsList { get; } = new ObservableCollection<BaseBoardDetail>();

        public OWBoard()
        {

        }

        public OWBoard(OWBaseBoard baseBoard)
        {
            _id = baseBoard.ID;
            _name = baseBoard.Name;
            _isAvailable = baseBoard.IsAvailable;
            _nativePeripheral = baseBoard.NativePeripheral;
        }

        public void Init()
        {
            App.Current.OWBLE.BoardValueChanged += OWBLE_BoardValueChanged;
            
            FullBoardDetailsList.Add(_serialNumber);
            FullBoardDetailsList.Add(_batterySerial);

            FullBoardDetailsList.Add(_batteryPercent);
            SelectedBoardDetailsList.Add(_batteryPercent);

            FullBoardDetailsList.Add(_batteryLow5);
            SelectedBoardDetailsList.Add(_batteryLow5);

            FullBoardDetailsList.Add(_batteryLow20);
            SelectedBoardDetailsList.Add(_batteryLow20);

            FullBoardDetailsList.Add(_rideMode);
            SelectedBoardDetailsList.Add(_rideMode);

            FullBoardDetailsList.Add(_pitch);
            FullBoardDetailsList.Add(_yaw);
            FullBoardDetailsList.Add(_roll);

            
            FullBoardDetailsList.Add(_rpm);
            SelectedBoardDetailsList.Add(_rpm);

            FullBoardDetailsList.Add(_speed);
            SelectedBoardDetailsList.Add(_speed);

            FullBoardDetailsList.Add(_statusError);
            SelectedBoardDetailsList.Add(_statusError);

            FullBoardDetailsList.Add(_controllerTemperature);
            SelectedBoardDetailsList.Add(_controllerTemperature);

            FullBoardDetailsList.Add(_motorTemperature);
            SelectedBoardDetailsList.Add(_motorTemperature);

            FullBoardDetailsList.Add(_firmwareRevision);
            SelectedBoardDetailsList.Add(_firmwareRevision);

            FullBoardDetailsList.Add(_currentAmps);
            SelectedBoardDetailsList.Add(_currentAmps);

            FullBoardDetailsList.Add(_tripAmpHours);
            SelectedBoardDetailsList.Add(_tripAmpHours);

            FullBoardDetailsList.Add(_tripRegenAmpHours);
            SelectedBoardDetailsList.Add(_tripRegenAmpHours);

            FullBoardDetailsList.Add(_unknownTemperature);
            SelectedBoardDetailsList.Add(_unknownTemperature);

            FullBoardDetailsList.Add(_batteryTemperature);
            SelectedBoardDetailsList.Add(_batteryTemperature);

            FullBoardDetailsList.Add(_batteryVoltage);
            SelectedBoardDetailsList.Add(_batteryVoltage);

            FullBoardDetailsList.Add(_safetyHeadroom);
            SelectedBoardDetailsList.Add(_safetyHeadroom);

            FullBoardDetailsList.Add(_lifetimeOdometer);
            SelectedBoardDetailsList.Add(_lifetimeOdometer);

            FullBoardDetailsList.Add(_lifetimeAmpHours);
            SelectedBoardDetailsList.Add(_lifetimeAmpHours);

            FullBoardDetailsList.Add(_lastErrorCode);
            SelectedBoardDetailsList.Add(_lastErrorCode);

            FullBoardDetailsList.Add(_batteryCells);
            SelectedBoardDetailsList.Add(_batteryCells);
           
        }


        private void OWBLE_BoardValueChanged(string characteristicGuid, byte[] data)
        {
            Debug.WriteLine($"{characteristicGuid} {BitConverter.ToString(data)}");

            if (_isHandshaking && characteristicGuid.Equals(SerialReadUUID, StringComparison.CurrentCultureIgnoreCase))
            {
                _handshakeBuffer.AddRange(data);
                if (_handshakeBuffer.Count == 20)
                {
                    _isHandshaking = false;
                    _handshakeTaskCompletionSource.SetResult(_handshakeBuffer.ToArray<byte>());
                }

                return;
            }


            SetValue(characteristicGuid, data);
        }

        

       

        // TODO: Restore, Dictionary<string, ICharacteristic> _characteristics = new Dictionary<string, ICharacteristic>();

        private void RSSIMonitor()
        {
            /*
            ThreadPool.QueueUserWorkItem(async (object state) =>
            {
                while (_device.State == Plugin.BLE.Abstractions.DeviceState.Connected)
                {
                    try
                    {
                        bool didUpdate = await _device.UpdateRssiAsync();
                        if (didUpdate)
                        {
                            RSSI = _device.Rssi;
                            if (_isLogging)
                            {
                                _events.BoardEvents.Add(new OWBoardEvent()
                                {
                                    Uuid = "RSSI",
                                    Data = ByteString.CopyFrom(BitConverter.GetBytes(_device.Rssi)),
                                    Timestamp = DateTime.Now.Ticks,
                                });
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        System.Diagnostics.Debug.WriteLine("RSSI fetch error: " + err.Message);
                    }
                    await Task.Delay(1000);
                }

            });
            */
        }

        internal async Task SubscribeToBLE()
        {
#if DEBUG
            if (_nativePeripheral == null)
                return;
#endif
            //RSSIMonitor();

            
            var characteristicsToReadNow = new List<string>()
            {
                SerialNumberUUID,
                BatteryPercentUUID,
                //BatteryLow5UUID,
                //BatteryLow20UUID,
                BatterySerialUUID,
                //PitchUUID,
                //RollUUID,
                //YawUUID,
                TripOdometerUUID,
                //RpmUUID,
                LightModeUUID,
                LightsFrontUUID,
                LightsBackUUID,
                //StatusErrorUUID,
                TemperatureUUID,
                FirmwareRevisionUUID,
                //CurrentAmpsUUID,
                TripAmpHoursUUID,
                TripRegenAmpHoursUUID,
                BatteryTemperatureUUID,
                BatteryVoltageUUID,
                SafetyHeadroomUUID,
                HardwareRevisionUUID,
                LifetimeOdometerUUID,
                LifetimeAmpHoursUUID,
                RideModeUUID,
                //BatteryCellsUUID,
                //LastErrorCodeUUID,
                //SerialRead,
                //SerialWrite,
                //UNKNOWN1UUID,
                //UNKNOWN2UUID,
                //UNKNOWN3UUID,
                //UNKNOWN4UUID,
            };

            var characteristicsToSubscribeTo = new List<string>()
            {
                //SerialNumberUUID,
                BatteryPercentUUID,
                BatteryLow5UUID,
                BatteryLow20UUID,
                //BatterySerialUUID,
                PitchUUID,
                RollUUID,
                YawUUID,
                TripOdometerUUID,
                RpmUUID,
                LightModeUUID,
                LightsFrontUUID,
                LightsBackUUID,
                StatusErrorUUID,
                TemperatureUUID,
                //FirmwareRevisionUUID,
                CurrentAmpsUUID,
                TripAmpHoursUUID,
                TripRegenAmpHoursUUID,
                BatteryTemperatureUUID,
                BatteryVoltageUUID,
                SafetyHeadroomUUID,
                RideModeUUID,
                //HardwareRevisionUUID,
                LifetimeOdometerUUID,
                LifetimeAmpHoursUUID,
                BatteryCellsUUID,
                LastErrorCodeUUID,
                //SerialRead,
                //SerialWrite,
                //UNKNOWN1UUID,
                //UNKNOWN2UUID,
                //UNKNOWN3UUID,
                //UNKNOWN4UUID,
            };

          
            var hardwareRevision = await App.Current.OWBLE.ReadValue(HardwareRevisionUUID);
            SetValue(HardwareRevisionUUID, hardwareRevision, true);
            var firmwareRevision = await App.Current.OWBLE.ReadValue(FirmwareRevisionUUID);
            SetValue(FirmwareRevisionUUID, firmwareRevision, true);


            if (_firmwareRevision.Value >= 5000)
            {
                MessagingCenter.Send<OWBoard>(this, "invalid_board_pint");
                return;
            }
            else if (_firmwareRevision.Value >= 4141)
            {
                MessagingCenter.Send<OWBoard>(this, "invalid_board_xr4141");
                return;
            }


            // Hide 16th cell on XR and Pint.
            if (_hardwareRevision.Value >= 4000)
            {
                _batteryCells.IgnoreCell(15);
            }

            if (_hardwareRevision.Value > 3000 && _firmwareRevision.Value > 4000)
            {
                await Handshake();
                _keepHandshakeBackgroundRunning = true;
                Device.StartTimer(TimeSpan.FromSeconds(15), () =>
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            byte[] firmwareRevision = GetBytesForBoardFromUInt16((UInt16)_firmwareRevision.Value, FirmwareRevisionUUID);
                            await App.Current.OWBLE.WriteValue(OWBoard.FirmwareRevisionUUID, firmwareRevision);
                        }
                        catch (Exception err)
                        {
                            // TODO: Couldnt update firmware revision.
                            Debug.WriteLine("ERROR: " + err.Message);
                        }
                    });
                    return _keepHandshakeBackgroundRunning;
                });
            }

            foreach (var characteristic in characteristicsToSubscribeTo)
            {
                //characteristic.ValueUpdated += Characteristic_ValueUpdated;
                await App.Current.OWBLE.SubscribeValue(characteristic);
            }

            foreach (var characteristic in characteristicsToReadNow)
            {
                var data = await App.Current.OWBLE.ReadValue(characteristic);
                var intValue = BitConverter.ToUInt16(data);

                if (characteristic == LifetimeOdometerUUID)
                {
                    int blahsda = 0;
                }
                SetValue(characteristic, data, true);
            }


            /*
            var readTasks = new Dictionary<string, Task<byte[]>>();
            //var readTask = new List<Task<byte[]>>();
           
            foreach (var characteristic in _characteristics)
            {
                var uuid = characteristic.u.ToUpper();
                if (characteristic.CanRead)
                {
                    //byte[] data = await char    acteristic.ReadAsync();

                    //Console.Write
                    //   readTasks.Add(charac              theTask = characteristic.ReadAsync();

                    readTasks.Add(uuid, characteristic.ReadAsync());
                    //theTask = characteristic.ReadAsync();
                }

                if (characteristic.CanUpdate)
                {
                    if (characteristicsToSubscribeTo.Contains(uuid))
                    {
                        characteristic.ValueUpdated += Characteristic_ValueUpdated;
                        await characteristic.StartUpdatesAsync();
                    }
                }
            }

            await Task.WhenAll(readTasks.Values.ToArray());


            foreach (var key in readTasks.Keys)
            {
                System.Diagnostics.Debug.WriteLine(key);
                SetValue(key, readTasks[key].Result, true);
            }
            */



            /*

            ReadRequestReceived - RideMode
CharacteristicSubscribed - RideMode
ReadRequestReceived - BatteryRemaining
CharacteristicSubscribed - BatteryRemaining
ReadRequestReceived - TiltAnglePitch
CharacteristicSubscribed - TiltAnglePitch
ReadRequestReceived - TiltAngleRoll
CharacteristicSubscribed - TiltAngleRoll
ReadRequestReceived - TiltAngleYaw
CharacteristicSubscribed - TiltAngleYaw
ReadRequestReceived - Odometer
CharacteristicSubscribed - Odometer
ReadRequestReceived - SpeedRpm
CharacteristicSubscribed - SpeedRpm
ReadRequestReceived - LightingMode
CharacteristicSubscribed - LightingMode
ReadRequestReceived - LightsFront
CharacteristicSubscribed - LightsFront
ReadRequestReceived - LightsBack
ReadRequestReceived - StatusError
CharacteristicSubscribed - StatusError
ReadRequestReceived - Temperature
ReadRequestReceived - FirmwareRevision
ReadRequestReceived - CurrentAmps
CharacteristicSubscribed - CurrentAmps
ReadRequestReceived - TripTotalAmpHours
CharacteristicSubscribed - TripTotalAmpHours
ReadRequestReceived - TripRegenAmpHours
CharacteristicSubscribed - TripRegenAmpHours
ReadRequestReceived - BatteryTemp
ReadRequestReceived - BatteryVoltage
CharacteristicSubscribed - BatteryVoltage
ReadRequestRe
        HardwareRevision
CharacteristicSubscribed - HardwareRevision
ReadRequestReceived - LifetimeOdometer
CharacteristicSubscribed - LifetimeOdometer
ReadRequestReceived - BatteryCells
Characteristic
        d - BatteryCells
ReadRequestReceived - LastErrorCode
CharacteristicSubscribed - LastErrorCode
ReadRequestReceived - LifetimeOdometer
*/
        }

        private List<byte> _handshakeBuffer = null;
        private bool _isHandshaking = false;
        private TaskCompletionSource<byte[]> _handshakeTaskCompletionSource = null;

        private async Task<bool> Handshake()
        {
            _isHandshaking = true;
            _handshakeTaskCompletionSource = new TaskCompletionSource<byte[]>();
            _handshakeBuffer = new List<byte>();

            //var rideMode = await _characteristics[OWBoard.RideModeUUID].ReadAsync();
            //await _characteristics[OWBoard.RideModeUUID].StartUpdatesAsync();

            //var batteryPercent = await _characteristics[OWBoard.BatteryPercentUUID].ReadAsync();
            //await _characteristics[OWBoard.BatteryPercentUUID].StartUpdatesAsync();

            //_characteristics[OWBoard.UNKNOWN1UUID].ValueUpdated += SerialRead_ValueUpdated;
            //_characteristics[OWBoard.UNKNOWN1UUID].ValueUpdated += SerialRead_ValueUpdated;
            // TODO: Restore _characteristics[OWBoard.SerialReadUUID].ValueUpdated += SerialRead_ValueUpdated;

            //await _characteristics[OWBoard.UNKNOWN1UUID].StartUpdatesAsync();
            //await _characteristics[OWBoard.UNKNOWN2UUID].StartUpdatesAsync();
            await App.Current.OWBLE.SubscribeValue(OWBoard.SerialReadUUID, true);

            // Data does not send until this is triggered. 
            byte[] firmwareRevision = GetBytesForBoardFromUInt16((UInt16)_firmwareRevision.Value, FirmwareRevisionUUID);

            var didWrite = await App.Current.OWBLE.WriteValue(OWBoard.FirmwareRevisionUUID, firmwareRevision, true);

            var byteArray = await _handshakeTaskCompletionSource.Task;
            
            await App.Current.OWBLE.UnsubscribeValue(OWBoard.SerialReadUUID, true);
            // TODO: Restore _characteristics[OWBoard.SerialReadUUID].ValueUpdated -= SerialRead_ValueUpdated;
            if (byteArray.Length == 20)
            {
                if (_firmwareRevision.Value >= 4141) // Pint or XR with 4210 hardware 
                {
                    /*
                    // Get bytes 3 through to 19 (start 3, length 16)
                    var apiKeyArray = new byte[16];
                    Array.Copy(byteArray, 3, apiKeyArray, 0, 16);

                    // Convert to base16 string.
                    var apiKey = BitConverter.ToString(apiKeyArray).Replace("-", "");

                    // Exchange this apiKey for key from server.
                    var tokenArray = await FetchToken(apiKey);
                    if (tokenArray != null)
                    {
                        // Feed it back to the app how we normally would.
                        await App.Current.OWBLE.WriteValue(OWBoard.SerialWriteUUID, tokenArray);
                    }
                    */
                }
                else
                {
                    var outputArray = new byte[20];
                    Array.Copy(byteArray, 0, outputArray, 0, 3);

                    // Take almost all of the bytes from the input array. This is almost the same as the last part as
                    // we are ignoring the first 3 and the last bytes.
                    var arrayToMD5_part1 = new byte[16];
                    Array.Copy(byteArray, 3, arrayToMD5_part1, 0, 16);

                    // This appears to be a static value from the board.
                    var arrayToMD5_part2 = new byte[] {
                        217,    // D9
                        37,     // 25
                        95,     // 5F
                        15,     // 0F
                        35,     // 23
                        53,     // 35
                        78,     // 4E
                        25,     // 19
                        186,    // BA
                        115,    // 73
                        156,    // 9C
                        205,    // CD
                        196,    // C4
                        169,    // A9
                        23,     // 17
                        101,    // 65
                    };


                    // New byte array we are going to MD5 hash. Part of the input string, part of this static string.
                    var arrayToMD5 = new byte[arrayToMD5_part1.Length + arrayToMD5_part2.Length];
                    arrayToMD5_part1.CopyTo(arrayToMD5, 0);
                    arrayToMD5_part2.CopyTo(arrayToMD5, arrayToMD5_part1.Length);

                    // Start prepping the MD5 hash
                    byte[] md5Hash = null;
                    using (var md5 = System.Security.Cryptography.MD5.Create())
                    {
                        md5Hash = md5.ComputeHash(arrayToMD5);
                    }

                    // Add it to the 3 bytes we already have.
                    Array.Copy(md5Hash, 0, outputArray, 3, md5Hash.Length);

                    // Validate the check byte.
                    outputArray[19] = 0;
                    for (int i = 0; i < outputArray.Length - 1; ++i)
                    {
                        outputArray[19] = ((byte)(outputArray[i] ^ outputArray[19]));
                    }

                    var inputString = BitConverter.ToString(byteArray).Replace("-", ":").ToLower();
                    var outputString = BitConverter.ToString(outputArray).Replace("-", ":").ToLower();

                    Debug.WriteLine($"Input: {inputString}");
                    Debug.WriteLine($"Output: {outputString}");

                    await App.Current.OWBLE.WriteValue(OWBoard.SerialWriteUUID, outputArray);
                }
            }
            return false;
        }

        /*
        private async Task<byte[]> FetchToken(string apiKey)
        {
            if (String.IsNullOrWhiteSpace(_name))
            {
                return null;
            }
            var deviceName = _name.ToLower();
            deviceName = deviceName.Replace("ow", String.Empty);
            var key = await SecureStorage.GetAsync($"board_{deviceName}_key");

            // If the API key has changed delete the stored token.
            if (key != apiKey)
            {
                SecureStorage.Remove($"board_{deviceName}_token");
            }

            var token = await SecureStorage.GetAsync($"board_{deviceName}_token");

            if (String.IsNullOrEmpty(token))
            {
                try
                {
                    using (var handler = new HttpClientHandler())
                    {
                        handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;

                        using (var client = new HttpClient())
                        {
                            // Match headers as best as possible.
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-us");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic Og==");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip");

                            var platform = DeviceInfo.Platform;
                            if (platform == DevicePlatform.Android)
                            {
                                var userAgent = DependencyService.Get<DependencyInterfaces.IUserAgent>();
                                var systemUserAgent = await userAgent.GetSystemUserAgent();
                                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", systemUserAgent);
                            }
                            else if (platform == DevicePlatform.iOS)
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Onewheel/0 CFNetwork/1121.2.2 Darwin/19.2.0");
                            }
                            else
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Onewheel/0 CFNetwork/1121.2.2 Darwin/19.2.0");
                            }
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                            string owType = _boardType switch
                            {
                                OWBoardType.XR => "xr",
                                OWBoardType.Pint => "pint",
                                _ => "",
                            };

                            // Request unlock key based on board name, board type, and token.
                            var url = $"https://app.onewheel.com/wp-json/fm/v2/activation/{deviceName}?owType={owType}&apiKey={apiKey}";
                            var response = await client.GetAsync(url);

                            if (response.IsSuccessStatusCode)
                            {
                                var responseBody = await response.Content.ReadAsStringAsync();
                                var owKey = JsonSerializer.Deserialize<OWKey>(responseBody);

                                if (String.IsNullOrWhiteSpace(owKey.Key))
                                {
                                    throw new Exception("No key found.");
                                }

                                await SecureStorage.SetAsync($"board_{deviceName}_key", apiKey);
                                await SecureStorage.SetAsync($"board_{deviceName}_token", owKey.Key);

                                var tokenArray = owKey.Key.StringToByteArray();
                                return tokenArray;
                            }
                            else
                            {
                                throw new Exception($"Unexpected response code ({response.StatusCode})");
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    Debug.WriteLine($"ERROR: {err.Message}");

                    SecureStorage.Remove($"board_{deviceName}_token");
                    SecureStorage.Remove($"board_{deviceName}_key");

                }
            }
            else
            {
                var tokenArray = token.StringToByteArray();
                return tokenArray;
            }

            return null;
        }
        */


        private byte[] GetBytesForBoardFromUInt16(UInt16 value, string uuidHint = null)
        {
            if (uuidHint == null)
            {

            }
            else
            {


            }

            var bytes = BitConverter.GetBytes(value);
            return bytes;
        }

        private void SetValue(string uuid, byte[] data, bool initialData = false)
        {
            if (data == null)
                return;

            uuid = uuid.ToUpper();


            if (initialData)
            {
                _initialEvents.Add(new OWBoardEvent()
                {
                    Uuid = uuid,
                    Data = ByteString.CopyFrom(data),
                    Timestamp = DateTime.UtcNow.Ticks,
                });
            }


            if (data.Length != 2)
                return;

            if (uuid == TemperatureUUID)
            {
                _motorTemperature.Value = data[0];
                _controllerTemperature.Value = data[1];

                return;
            }
            else if (uuid == BatteryTemperatureUUID)
            {
                _unknownTemperature.Value = data[0];
                _batteryTemperature.Value = data[1];

                return;
            }
            else if (uuid == BatteryPercentUUID)
            {
                if (data[0] > 0)
                {
                    _batteryPercent.Value = data[0];
                }
                else if (data[1] > 0)
                {
                    _batteryPercent.Value = data[1];
                }
            }


            var value = BitConverter.ToUInt16(data, 0);


            if (_isLogging)
            {
                //  var boardEvent = new OWBoardEvent();
                // boardEvent.write
                //boardEvent.WriteDelimitedTo()
                _events.BoardEvents.Add(new OWBoardEvent()
                {
                    Uuid = uuid,
                    Data = ByteString.CopyFrom(data),
                    Timestamp = DateTime.UtcNow.Ticks,
                });

                if (_events.BoardEvents.Count > 1000)
                {
                    SaveEvents();
                }
            }

            switch (uuid)
            {
                case SerialNumberUUID:
                    _serialNumber.Value = value;
                    break;
                case BatteryLow5UUID:
                    _batteryLow5.Value = value;
                    break;
                case BatteryLow20UUID:
                    _batteryLow20.Value = value;
                    break;
                case BatterySerialUUID:
                    _batterySerial.Value = value;
                    break;
                case PitchUUID:
                    _pitch.Value = 0.1f * (1800 - value);
                    break;
                case RollUUID:
                    _roll.Value = 0.1f * (1800 - value);
                    break;
                case YawUUID:
                    _yaw.Value = 0.1f * (1800 - value);
                    break;
                case TripOdometerUUID:
                    _tripOdometer.Value = value;
                    break;
                case RpmUUID:
                    if (_rpm.Value != value)
                    { 
                        _rpm.Value = value;
    
                        var speedInMetersPerSecond = _tyreRadius * RadConvert * _rpm.Value;
                        _speed.Value = speedInMetersPerSecond;
                    }

                    break;
                case RideModeUUID:
                    _rideMode.Value = value;

                    // Fire these updates as well.
                    OnPropertyChanged("MaxDisplaySpeed");
                    OnPropertyChanged("MaxRecommendedSpeed");
                    break;
                case LightModeUUID:
                    LightMode = (value == 1);
                    break;
                case LightsFrontUUID:
                    FrontLightMode = value;
                    break;
                case LightsBackUUID:
                    RearLightMode = value;
                    break;
                case StatusErrorUUID:
                    _statusError.Value = value;
                    break;
                case FirmwareRevisionUUID:
                    _firmwareRevision.Value = value;
                    break;
                case CurrentAmpsUUID:
                    _currentAmps.Value = 0.1f * value;
                    break;
                case TripAmpHoursUUID:
                    _tripAmpHours.Value = 0.1f * value;
                    break;
                case TripRegenAmpHoursUUID:
                    _tripRegenAmpHours.Value = 0.1f * value;
                    break;
                case BatteryVoltageUUID:
                    _batteryVoltage.Value = 0.1f * value;
                    break;
                case SafetyHeadroomUUID:
                    _safetyHeadroom.Value = value;
                    break;
                case HardwareRevisionUUID:
                    _hardwareRevision.Value = value;

                    if (value >= 1 && value <= 2999)
                    {
                        BoardType = OWBoardType.V1;
                    }
                    else if (value >= 3000 && value <= 3999)
                    {
                        BoardType = OWBoardType.Plus;
                    }
                    else if (value >= 4000 && value <= 4999)
                    {
                        BoardType = OWBoardType.XR;
                    }
                    else if (value >= 5000 && value <= 5999)
                    {
                        BoardType = OWBoardType.Pint;
                    }

                    break;
                case LifetimeOdometerUUID:
                    _lifetimeOdometer.Value = value;
                    break;
                case LifetimeAmpHoursUUID:
                    _lifetimeAmpHours.Value = value;
                    break;
                case BatteryCellsUUID:

                    var batteryVoltage = (uint)data[0];
                    var cellID = (uint)data[1];
                    //var batteryVoltageDisplay = batteryVoltage / 50.0;

                    _batteryCells.SetCell(cellID, batteryVoltage);

                    //batteryVoltageCells[cellIdentifier] = (double)var3 / 50.0D;
                    //Debug.WriteLine($"BatteryCellsUUID: {cellID} {batteryVoltage} {batteryVoltageDisplay} ");

                    break;
                case LastErrorCodeUUID:

                    break;
                case UNKNOWN1UUID:
                    _UNKNOWN1 = value;
                    break;
                case UNKNOWN2UUID:
                    _UNKNOWN2 = value;
                    break;
                case UNKNOWN3UUID:
                    _UNKNOWN3 = value;
                    break;
                case UNKNOWN4UUID:
                    _UNKNOWN4 = value;
                    break;
            }
        }

        /*Disconnect
        void SerialRead_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            string uuid = e.Characteristic.Uuid.ToUpper();
            Debug.WriteLine($"SerialRead_ValueUpdated - {uuid}");
            if (_isHandshaking && uuid == SerialReadUUID)
            {
                _handshakeBuffer.AddRange(e.Characteristic.Value);
                if (_handshakeBuffer.Count == 20)
                {
                    _isHandshaking = false;
                    _handshakeTaskCompletionSource.SetResult(_handshakeBuffer.ToArray<byte>());
                }
            }
        }

        void Characteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            string uuid = e.Characteristic.Uuid.ToUpper();
            SetValue(uuid, e.Characteristic.Value);
        }
        */

        public async Task Disconnect()
        {
            App.Current.OWBLE.BoardValueChanged -= OWBLE_BoardValueChanged;
            _keepHandshakeBackgroundRunning = false;
            await App.Current.OWBLE.Disconnect();
            //await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(_device);
        }

        private bool _isLogging = false;
        //private long _currentRunStart = 0;
        //public long CurrentRunStart { get{ return _currentRunStart; } }

        public async Task StartLogging()
        {
            // _currentRunStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            //_logDirectory = Path.Combine(FileSystem.CacheDirectory, _currentRunStart.ToString());
            var currentRunStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _currentRide = new Ride($"{currentRunStart}.dat");


            _isLogging = true;
            _events = new OWBoardEventList();
            _events.BoardEvents.AddRange(_initialEvents);


            /*
            if (CrossGeolocator.Current.IsGeolocationAvailable)
            {
                CrossGeolocator.Current.DesiredAccuracy = 1;

                await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true);

                CrossGeolocator.Current.PositionChanged += PositionChanged;
                CrossGeolocator.Current.PositionError += PositionError;

            }
            */
        }

        public async Task<string> StopLogging()
        {
            _isLogging = false;
            _currentRide.EndTime = DateTime.Now;


            Hud.Show("Compressing data");
            /*
            await CrossGeolocator.Current.StopListeningAsync(); ;

            CrossGeolocator.Current.PositionChanged -= PositionChanged;
            CrossGeolocator.Current.PositionError -= PositionError;
            */

            SaveEvents();


            var logFilePath = _currentRide.GetLogFilePath();
            string datFileName = Path.GetFileName(logFilePath);
            var zipPath = Path.Combine(FileSystem.CacheDirectory, $"{datFileName}.zip");

            using (FileStream fs = File.Create(zipPath))
            {
                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fs))
                {
                    zipStream.SetLevel(3);

                    //FileInfo inFileInfo = new FileInfo(inputPath);


                    ICSharpCode.SharpZipLib.Zip.ZipEntry newEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry(datFileName);
                    newEntry.DateTime = DateTime.UtcNow;
                    zipStream.PutNextEntry(newEntry);

                    byte[] buffer = new byte[4096];
                    using (FileStream streamReader = File.OpenRead(logFilePath))
                    {
                        ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(streamReader, zipStream, buffer);
                    }

                    zipStream.CloseEntry();
                    zipStream.IsStreamOwner = true;
                    zipStream.Close();
                }
            }


            return zipPath;
        }


        /*
        double _oldLat = 0;
        double _oldLon = 0;
        private void PositionChanged(object sender, PositionEventArgs e)
        {
            if (_oldLat.Equals(e.Position.Latitude) == false || _oldLon.Equals(e.Position.Longitude) == false)
            {
                _oldLat = e.Position.Latitude;
                _oldLon = e.Position.Longitude;

                _events.BoardEvents.Add(new OWBoardEvent()
                {
                    Uuid = "gps_latitude",
                    Data = ByteString.CopyFrom(BitConverter.GetBytes(e.Position.Latitude)),
                    Timestamp = DateTime.UtcNow.Ticks,
                });

                _events.BoardEvents.Add(new OWBoardEvent()
                {
                    Uuid = "gps_longitude",
                    Data = ByteString.CopyFrom(BitConverter.GetBytes(e.Position.Longitude)),
                    Timestamp = DateTime.UtcNow.Ticks,
                });

                _events.BoardEvents.Add(new OWBoardEvent()
                {
                    Uuid = "gps_altitude",
                    Data = ByteString.CopyFrom(BitConverter.GetBytes(e.Position.Altitude)),
                    Timestamp = DateTime.UtcNow.Ticks,
                });

                _events.BoardEvents.Add(new OWBoardEvent()
                {
                    Uuid = "gps_speed",
                    Data = ByteString.CopyFrom(BitConverter.GetBytes(e.Position.Speed)),
                    Timestamp = DateTime.UtcNow.Ticks,
                });

                _events.BoardEvents.Add(new OWBoardEvent()
                {
                    Uuid = "gps_accuracy",
                    Data = ByteString.CopyFrom(BitConverter.GetBytes(e.Position.Accuracy)),
                    Timestamp = DateTime.UtcNow.Ticks,
                });

                _events.BoardEvents.Add(new OWBoardEvent()
                {
                    Uuid = "gps_heading",
                    Data = ByteString.CopyFrom(BitConverter.GetBytes(e.Position.Heading)),
                    Timestamp = DateTime.UtcNow.Ticks,
                });

                //If updating the UI, ensure you invoke on main thread
                var position = e.Position;
                var output = "Full: Lat: " + position.Latitude + " Long: " + position.Longitude;
                output += "\n Full: Lat: " + ((float)position.Latitude) + " Long: " + ((float)position.Longitude);
                output += "\n" + $"Time: {position.Timestamp}";
                output += "\n" + $"Heading: {position.Heading}";
                output += "\n" + $"Speed: {position.Speed}";
                output += "\n" + $"Accuracy: {position.Accuracy}";
                output += "\n" + $"Altitude: {position.Altitude}";
                output += "\n" + $"Altitude Accuracy: {position.AltitudeAccuracy}";
                System.Diagnostics.Debug.WriteLine(output);
            }
        }

        private void PositionError(object sender, PositionErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Error);
            //Handle event here for errors
        }
        */

        private void SaveEvents()
        {
            try
            {
                var oldEvents = _events;
                _events = new OWBoardEventList();

                using (FileStream fs = new FileStream(_currentRide.GetLogFilePath(), FileMode.Append, FileAccess.Write))
                {
                    foreach (var owBoardEvent in oldEvents.BoardEvents)
                    {
                        owBoardEvent.WriteDelimitedTo(fs);
                    }

                    /*
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(myNewCSVLine);
                    }
                    */
                }
                //long currentRunEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                // var outputFile = Path.Combine(_logDirectory, $"{currentRunEnd}.dat");
                /*
                using (var output = File.Create(outputFile))
                {
                    oldEvents.WriteTo(output);
                }
                */
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: " + err.Message);
            }
        }

        public static string GetNameFromUUID(string uuid)
        {
            uuid = uuid.ToUpper();

            switch (uuid)
            {
                case SerialNumberUUID:
                    return "SerialNumber";
                case RideModeUUID:
                    return "RideMode";
                case BatteryPercentUUID:
                    return "BatteryPercent";
                case BatteryLow5UUID:
                    return "BatteryLow5";
                case BatteryLow20UUID:
                    return "BatteryLow20";
                case BatterySerialUUID:
                    return "BatterySerial";
                case PitchUUID:
                    return "Pitch";
                case RollUUID:
                    return "Roll";
                case YawUUID:
                    return "Yaw";
                case TripOdometerUUID:
                    return "TripOdometer";
                case RpmUUID:
                    return "Rpm";
                case LightModeUUID:
                    return "LightMode";
                case LightsFrontUUID:
                    return "LightsFront";
                case LightsBackUUID:
                    return "LightsBack";
                case StatusErrorUUID:
                    return "StatusError";
                case TemperatureUUID:
                    return "Temperature";
                case FirmwareRevisionUUID:
                    return "FirmwareRevision";
                case CurrentAmpsUUID:
                    return "CurrentAmps";
                case TripAmpHoursUUID:
                    return "TripAmpHours";
                case TripRegenAmpHoursUUID:
                    return "TripRegenAmpHours";
                case BatteryTemperatureUUID:
                    return "BatteryTemperature";
                case BatteryVoltageUUID:
                    return "BatteryVoltage";
                case SafetyHeadroomUUID:
                    return "SafetyHeadroom";
                case HardwareRevisionUUID:
                    return "HardwareRevision";
                case LifetimeOdometerUUID:
                    return "LifetimeOdometer";
                case LifetimeAmpHoursUUID:
                    return "LifetimeAmpHours";
                case BatteryCellsUUID:
                    return "BatteryCells";
                case LastErrorCodeUUID:
                    return "LastErrorCode";
                case SerialReadUUID:
                    return "SerialRead";
                case SerialWriteUUID:
                    return "SerialWrite";
                case UNKNOWN1UUID:
                    return "UNKNOWN1";
                case UNKNOWN2UUID:
                    return "UNKNOWN2";
                case UNKNOWN3UUID:
                    return "UNKNOWN3";
                case UNKNOWN4UUID:
                    return "UNKNOWN4";
            }

            return "Unknown";
        }
    }

    internal class OWKey
    {
        [System.Text.Json.Serialization.JsonPropertyName("key")]
        public string Key
        {
            get; set;
        }
    }
}
