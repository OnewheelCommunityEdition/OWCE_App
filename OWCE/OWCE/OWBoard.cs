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
using OWCE.Network;
using OWCE.DependencyInterfaces;
using Rg.Plugins.Popup.Services;
using MvvmHelpers;
using OWCE.PropertyChangeHandlers;

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

    public struct RideModes
    {
        public const int V1_Classic = 1;
        public const int V1_Extreme = 2;
        public const int V1_Elevated = 3;
        public const int PlusXR_Sequoia = 4;
        public const int PlusXR_Cruz = 5;
        public const int PlusXR_Mission = 6;
        public const int PlusXR_Elevated = 7;
        public const int PlusXR_Delirium = 8;
        public const int PlusXR_Custom = 9;
        public const int Pint_Redwood = 5;
        public const int Pint_Pacific = 6;
        public const int Pint_Elevated = 7;
        public const int Pint_Skyline = 8;
    }

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

        private int _serialNumber;
        public int SerialNumber
        {
            get { return _serialNumber; }
            set { if (_serialNumber != value) { _serialNumber = value; OnPropertyChanged(); } }
        }

        private int _batteryPercent;
        public int BatteryPercent
        {
            get { return _batteryPercent; }
            set { if (_batteryPercent != value) { _batteryPercent = value; OnPropertyChanged(); } }
        }

        private int _batteryLow5;
        public int BatteryLow5
        {
            get { return _batteryLow5; }
            set { if (_batteryLow5 != value) { _batteryLow5 = value; OnPropertyChanged(); } }
        }

        private int _batteryLow20;
        public int BatteryLow20
        {
            get { return _batteryLow20; }
            set { if (_batteryLow20 != value) { _batteryLow20 = value; OnPropertyChanged(); } }
        }

        private int _batterySerial;
        public int BatterySerial
        {
            get { return _batterySerial; }
            set { if (_batterySerial != value) { _batterySerial = value; OnPropertyChanged(); } }
        }

        float _pitch;
        public float Pitch
        {
            get { return _pitch; }
            set { if (_pitch.AlmostEqualTo(value) == false) { _pitch = value; OnPropertyChanged(); } }
        }

        float _yaw;
        public float Yaw
        {
            get { return _yaw; }
            set { if (_yaw.AlmostEqualTo(value) == false) { _yaw = value; OnPropertyChanged(); } }
        }

        float _roll;
        public float Roll
        {
            get { return _roll; }
            set { if (_roll.AlmostEqualTo(value) == false) { _roll = value; OnPropertyChanged(); } }
        }

        /*
        float _yyy;
        public float YYY
        {
            get { return _yyy; }
            set { if (_yyy.AlmostEqualTo(value) == false) { _yyy = value; OnPropertyChanged(); } }
        }

        private int _xxx;
        public int XXX
        {
            get { return _xxx; }
            set { if (_xxx != value) { _xxx = value; OnPropertyChanged(); } }
        }
        */



        private ushort _tripOdometer;
        public ushort TripOdometer
        {
            get { return _tripOdometer; }
            set { if (_tripOdometer != value) { _tripOdometer = value; OnPropertyChanged(); } }
        }

        private int _statusError;
        public int StatusError
        {
            get { return _statusError; }
            set { if (_statusError != value) { _statusError = value; OnPropertyChanged(); } }
        }

        float _controllerTemperature;
        public float ControllerTemperature
        {
            get { return _controllerTemperature; }
            set { if (_controllerTemperature.AlmostEqualTo(value) == false) { _controllerTemperature = value; OnPropertyChanged(); } }
        }


        float _motorTemperature;
        public float MotorTemperature
        {
            get { return _motorTemperature; }
            set { if (_motorTemperature.AlmostEqualTo(value) == false) { _motorTemperature = value; OnPropertyChanged(); } }
        }

        private int _firmwareRevision;
        public int FirmwareRevision
        {
            get { return _firmwareRevision; }
            set { if (_firmwareRevision != value) { _firmwareRevision = value; OnPropertyChanged(); } }
        }

        float _currentAmps;
        public float CurrentAmps
        {
            get { return _currentAmps; }
            set { if (_currentAmps.AlmostEqualTo(value) == false) { _currentAmps = value; OnPropertyChanged(); } }
        }

        bool _isRegen;
        public bool IsRegen
        {
            get { return _isRegen; }
            set { if (_isRegen != value) { _isRegen = value; OnPropertyChanged(); } }
        }


        float _tripAmpHours;
        public float TripAmpHours
        {
            get { return _tripAmpHours; }
            set { if (_tripAmpHours.AlmostEqualTo(value) == false) { _tripAmpHours = value; OnPropertyChanged(); } }
        }

        float _tripRegenAmpHours;
        public float TripRegenAmpHours
        {
            get { return _tripRegenAmpHours; }
            set { if (_tripRegenAmpHours.AlmostEqualTo(value) == false) { _tripRegenAmpHours = value; OnPropertyChanged(); } }
        }

        float _batteryTemperature;
        public float BatteryTemperature
        {
            get { return _batteryTemperature; }
            set { if (_batteryTemperature.AlmostEqualTo(value) == false) { _batteryTemperature = value; OnPropertyChanged(); } }
        }

        float _batteryVoltage;
        public float BatteryVoltage
        {
            get { return _batteryVoltage; }
            set { if (_batteryVoltage.AlmostEqualTo(value) == false) { _batteryVoltage = value; OnPropertyChanged(); } }
        }

        private int _safetyHeadroom;
        public int SafetyHeadroom
        {
            get { return _safetyHeadroom; }
            set { if (_safetyHeadroom != value) { _safetyHeadroom = value; OnPropertyChanged(); } }
        }

        float _lifetimeOdometer;
        public float LifetimeOdometer
        {
            get { return _lifetimeOdometer; }
            set { if (_lifetimeOdometer.AlmostEqualTo(value) == false) { _lifetimeOdometer = value; OnPropertyChanged(); } }
        }

        float _lifetimeAmpHours;
        public float LifetimeAmpHours
        {
            get { return _lifetimeAmpHours; }
            set { if (_lifetimeAmpHours.AlmostEqualTo(value) == false) { _lifetimeAmpHours = value; OnPropertyChanged(); } }
        }

        float _lastErrorCode;
        public float LastErrorCode
        {
            get { return _lastErrorCode; }
            set { if (_lastErrorCode.AlmostEqualTo(value) == false) { _lastErrorCode = value; OnPropertyChanged(); } }
        }

        BatteryCells _batteryCells = new BatteryCells();
        public BatteryCells BatteryCells
        {
            get { return _batteryCells; }
        }

        private int _rpm;
        public int RPM
        {
            get { return _rpm; }
            set { if (_rpm != value) { _rpm = value; OnPropertyChanged(); } }
        }

        // Value is stored in meters per second.
        float _speed;
        public float Speed
        {
            get { return _speed; }
            set { if (_speed.AlmostEqualTo(value) == false) { _speed = value; OnPropertyChanged(); } }
        }

        private ushort _hardwareRevision;
        public ushort HardwareRevision
        {
            get { return _hardwareRevision; }
            set { if (_hardwareRevision != value) { _hardwareRevision = value; OnPropertyChanged(); } }
        }

        private ushort _rideMode;
        public ushort RideMode
        {
            get { return _rideMode; }
            set { if (_rideMode != value) { _rideMode = value; OnPropertyChanged(); OnPropertyChanged("RideModeString"); } }
        }

        private ushort _incomingRideMode;
        public ushort IncomingRideMode
        {
            get { return _incomingRideMode; }
            set { if (_incomingRideMode != value) { _incomingRideMode = value; OnPropertyChanged(); } }
        }

        public string RideModeString
        {
            get
            {
                if (_boardType == OWBoardType.V1)
                {
                    return _rideMode switch
                    {
                        1 => "Classic",
                        2 => "Extreme",
                        3 => "Elevated",
                        _ => "Unknown",
                    };
                }
                else if (_boardType == OWBoardType.Plus || _boardType == OWBoardType.XR)
                {
                    return _rideMode switch
                    {
                        4 => "Sequoia",
                        5 => "Cruz",
                        6 => "Mission",
                        7 => "Elevated",
                        8 => "Delirium",
                        9 => "Custom",
                        _ => "Unknown",
                    };
                }
                else if (_boardType == OWBoardType.Pint)
                {
                    return _rideMode switch
                    {
                        5 => "Redwood",
                        6 => "Pacific",
                        7 => "Elevated",
                        8 => "Skyline",
                        _ => "Unknown",
                    };
                }

                return "Unknown";
            }
        }

        private bool? _simpleStopEnabled = null;
        public bool? SimpleStopEnabled
        {
            get { return _simpleStopEnabled; }
            set { if (_simpleStopEnabled != value) { _simpleStopEnabled = value; OnPropertyChanged(); } }
        }

        /*
        public int MaxRecommendedSpeed
        {
            get
            {
                if (App.Current.MetricDisplay)
                {
                    if (_boardType == OWBoardType.V1)
                    {
                        return _rideMode switch
                        {
                            1 => 19, // Classic
                            2 => 24, // Extreme
                            3 => 24, // Elevated
                            _ => 24, // Unknown
                        };
                    }
                    else if (_boardType == OWBoardType.Plus || _boardType == OWBoardType.XR)
                    {
                        return _rideMode switch
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
                        return _rideMode switch
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
                        return _rideMode switch
                        {
                            1 => 12, // Classic
                            2 => 15, // Extreme
                            3 => 15, // Elevated
                            _ => 15, // Unknown
                        };
                    }
                    else if (_boardType == OWBoardType.Plus || _boardType == OWBoardType.XR)
                    {
                        return _rideMode switch
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
                        return _rideMode switch
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
        */





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

        int _rssi = 0;
        public int RSSI
        {
            get { return _rssi; }
            set { if (_rssi != value) { _rssi = value; OnPropertyChanged(); } }
        }

        IOWBLE _owble;

        bool _isLogging = false;
        OWBoardEventList _events = new OWBoardEventList();
        List<OWBoardEvent> _initialEvents;
        Ride _currentRide = null;
        bool _keepHandshakeBackgroundRunning = false;
        List<byte> _handshakeBuffer = null;
        bool _isHandshaking = false;
        TaskCompletionSource<byte[]> _handshakeTaskCompletionSource = null;

        public OWBoard(IOWBLE owble, OWBaseBoard baseBoard) : base(baseBoard)
        {
            MessagingCenter.Subscribe<App>(this, App.UnitDisplayUpdatedKey, (app) =>
            {
                OnPropertyChanged(nameof(RPM));
                OnPropertyChanged(nameof(LifetimeOdometer));
                OnPropertyChanged(nameof(TripOdometer));
            });

            _owble = owble;
            _id = baseBoard.ID;
            _name = baseBoard.Name;
            _isAvailable = baseBoard.IsAvailable;
            _nativePeripheral = baseBoard.NativePeripheral;
            _owble.BoardValueChanged += OWBLE_BoardValueChanged;
            _owble.RSSIUpdated += OWBLE_RSSIUpdated;

            // Subscribe to property changes to keep watch app in sync
            // (eg speed, battery percent changes)
            this.PropertyChanged += WatchSyncEventHandler.HandlePropertyChanged;

            MessagingCenter.Subscribe<object>(this, "start_recording", (source) =>
            {
                if (_isLogging)
                    return;

                StartLogging();
            });
            MessagingCenter.Subscribe<object>(this, "stop_recording", (source) =>
            {
                if (_isLogging)
                {
                    StopLogging();
                }
            });
        }

        public virtual void Init()
        {
#if DEBUG
            if (DeviceInfo.DeviceType == DeviceType.Physical)
            {
                StartLogging();
            }
#endif
        }

        void LogData(string characteristicGuid, byte[] data)
        {
            var byteString = ByteString.CopyFrom(data);
#if DEBUG
            // Remove serials from debug builds recording data.
            if (characteristicGuid.Equals(SerialNumberUUID, StringComparison.InvariantCultureIgnoreCase))
            {
                byteString = ByteString.CopyFrom(BitConverter.GetBytes(123456));
            }
            else if (characteristicGuid.Equals(BatterySerialUUID, StringComparison.InvariantCultureIgnoreCase))
            {
                byteString = ByteString.CopyFrom(BitConverter.GetBytes(789123));
            }
#endif

            _events.BoardEvents.Add(new OWBoardEvent()
            {
                Uuid = characteristicGuid,
                Data = ByteString.CopyFrom(data),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            });

            if (_events.BoardEvents.Count > 1000)
            {
                SaveEvents();
            }
        }

        private void OWBLE_BoardValueChanged(string characteristicGuid, byte[] data)
        {
            //Debug.WriteLine($"{characteristicGuid} {BitConverter.ToString(data)}");

            if (_isLogging)
            {
                LogData(characteristicGuid, data);
            }


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

        private void OWBLE_RSSIUpdated(int rssi)
        {
            RSSI = rssi;
        }


        // TODO: Restore, Dictionary<string, ICharacteristic> _characteristics = new Dictionary<string, ICharacteristic>();

        private void RSSIMonitor()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
            {
                try
                {
                    App.Current.OWBLE.RequestRSSIUpdate();
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine("RSSI fetch error: " + err.Message);
                }
                return true;
            });
        }

        internal async Task SubscribeToBLE()
        {
#if DEBUG
            if (_nativePeripheral == null)
                return;
#endif
            RSSIMonitor();


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
                RpmUUID,
                LightModeUUID,
                LightsFrontUUID,
                LightsBackUUID,
                //StatusErrorUUID,
                TemperatureUUID,
                //FirmwareRevisionUUID,
                CurrentAmpsUUID,
                TripAmpHoursUUID,
                TripRegenAmpHoursUUID,
                BatteryTemperatureUUID,
                BatteryVoltageUUID,
                //SafetyHeadroomUUID,
                //HardwareRevisionUUID,
                LifetimeOdometerUUID,
                LifetimeAmpHoursUUID,
                RideModeUUID,
                //BatteryCellsUUID,
                LastErrorCodeUUID,
                //SerialRead,
                //SerialWrite,
                //UNKNOWN1UUID,
                //UNKNOWN2UUID,
                //UNKNOWN3UUID,
                //UNKNOWN4UUID,
            };

            // Android can subscribe up to 15 things at once.
            var characteristicsToSubscribeTo = new List<string>()
            {
                //SerialNumberUUID,
                BatteryPercentUUID,
                //BatteryLow5UUID,
                //BatteryLow20UUID,
                //BatterySerialUUID,
                //PitchUUID,
                //RollUUID,
                //YawUUID,
                TripOdometerUUID,
                RpmUUID,
                //LightModeUUID,
                //LightsFrontUUID,
                //LightsBackUUID,
                StatusErrorUUID,
                TemperatureUUID,
                //FirmwareRevisionUUID,
                CurrentAmpsUUID,
                TripAmpHoursUUID,
                TripRegenAmpHoursUUID,
                BatteryTemperatureUUID,
                BatteryVoltageUUID,
                //SafetyHeadroomUUID,
                RideModeUUID,
                //HardwareRevisionUUID,
                //LifetimeOdometerUUID,
                //LifetimeAmpHoursUUID,
                BatteryCellsUUID,
                //LastErrorCodeUUID,
                //SerialRead,
                //SerialWrite,
                //UNKNOWN1UUID,
                //UNKNOWN2UUID,
                //UNKNOWN3UUID,
                //UNKNOWN4UUID,
            };


            var hardwareRevision = await _owble.ReadValue(HardwareRevisionUUID);
            SetValue(HardwareRevisionUUID, hardwareRevision, true);
            var firmwareRevision = await _owble.ReadValue(FirmwareRevisionUUID);
            SetValue(FirmwareRevisionUUID, firmwareRevision, true);

            if (HardwareRevision > 3000 && FirmwareRevision > 4000) // Requires Gemini handshake
            {
                var rideMode = await _owble.ReadValue(RideModeUUID);
                var rideModeInt = BitConverter.ToUInt16(rideMode, 0);

                if (rideModeInt > 0)
                {
                    // NOOP: Board is active 😜
                }
                else if (FirmwareRevision >= 4142) // Pint or XR with 4210 hardware 
                {
                    if (FirmwareRevision >= 4155 && HardwareRevision < 5000) // XR with 4155 FW.
                    {
                        await App.Current.MainPage.DisplayAlert("Oh no!", "Some features of this app currently will not work with board firmware 4155 and higher.\n\nFuture Motion has locked some features down and as a result prevents apps like OWCE reporting valuable data to you.\n\nSorry about that.", "Ok");
                    }

                    // No longer using the handshake with web connection.
                    var jumpstartAlert = new Pages.Popup.JumpstartAlert(new Command(async () =>
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                        if (App.Current.MainPage.Navigation.ModalStack.Count == 1 && App.Current.MainPage.Navigation.ModalStack.FirstOrDefault() is NavigationPage modalNavigationPage && modalNavigationPage.CurrentPage is Pages.BoardPage boardPage)
                        {
                            await boardPage.DisconnectAndPop();
                            return;
                        }
                    }));
                    await PopupNavigation.Instance.PushAsync(jumpstartAlert, true);
                    return;
                }
                else // XR 4209 and below
                {
                    try
                    {
                        await Handshake();
                    }
                    catch (Exceptions.HandshakeException handshakeException)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", handshakeException.Message, "Ok");
                        if (handshakeException.ShouldDisconnect)
                        {
                            if (App.Current.MainPage.Navigation.ModalStack.Count == 1 && App.Current.MainPage.Navigation.ModalStack.FirstOrDefault() is Pages.CustomNavigationPage modalNavigationPage && modalNavigationPage.CurrentPage is Pages.BoardPage boardPage)
                            {
                                await boardPage.DisconnectAndPop();
                            }
                            return;
                        }
                    }
                }

                // Turns out the below timer does not fire immedaitly, it fires after the first 15sec have passed.
                // Calling this before we start the timer should make it work more reliably.
                KeepBoardAlive().SafeFireAndForget();

                _keepHandshakeBackgroundRunning = true;
                Device.StartTimer(TimeSpan.FromSeconds(15), () =>
                {
                    KeepBoardAlive().SafeFireAndForget();
                    return _keepHandshakeBackgroundRunning;
                });
            }

            foreach (var characteristic in characteristicsToSubscribeTo)
            {
                await _owble.SubscribeValue(characteristic);
            }

            foreach (var characteristic in characteristicsToReadNow)
            {
                var data = await _owble.ReadValue(characteristic);
                SetValue(characteristic, data, true);
            }

        }

        // This should be called to keep the board in its unlocked state.
        async Task KeepBoardAlive()
        {
            try
            {
                byte[] firmwareRevision = GetBytesForBoardFromUInt16((UInt16)FirmwareRevision, FirmwareRevisionUUID);
                await _owble.WriteValue(OWBoard.FirmwareRevisionUUID, firmwareRevision);
            }
            catch (Exception err)
            {
                // TODO: Couldnt update firmware revision.
                Debug.WriteLine("ERROR: " + err.Message);
            }
        }

        private async Task<bool> Handshake()
        {
            _isHandshaking = true;
            _handshakeTaskCompletionSource = new TaskCompletionSource<byte[]>();
            _handshakeBuffer = new List<byte>();

            await _owble.SubscribeValue(OWBoard.SerialReadUUID, true);

            // Data does not send until this is triggered. 
            byte[] firmwareRevision = GetBytesForBoardFromUInt16((UInt16)FirmwareRevision, FirmwareRevisionUUID);

            var didWrite = await _owble.WriteValue(OWBoard.FirmwareRevisionUUID, firmwareRevision, true);

            var byteArray = await _handshakeTaskCompletionSource.Task;

            await _owble.UnsubscribeValue(OWBoard.SerialReadUUID, true);
            // TODO: Restore _characteristics[OWBoard.SerialReadUUID].ValueUpdated -= SerialRead_ValueUpdated;
            if (byteArray.Length == 20)
            {
                if (FirmwareRevision >= 4141) // Pint or XR with 4210 hardware 
                {
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
                        await _owble.WriteValue(OWBoard.SerialWriteUUID, tokenArray);
                    }
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

                    await _owble.WriteValue(OWBoard.SerialWriteUUID, outputArray);
                }
            }
            return false;
        }

        private async Task<byte[]> FetchToken(string apiKey)
        {
            if (String.IsNullOrWhiteSpace(_name))
            {
                return null;
            }
            var deviceName = _name.ToLower();
            deviceName = deviceName.Replace("ow", String.Empty);


            //SecureStorage.Remove($"board_{deviceName}_token");
            //SecureStorage.Remove($"board_{deviceName}_key");

            var key = await SecureStorage.GetAsync($"board_{deviceName}_key");

            // If the API key has changed delete the stored token.
            if (key != apiKey)
            {
                SecureStorage.Remove($"board_{deviceName}_token");
            }


            // If we already have a token lets use it.
            var token = await SecureStorage.GetAsync($"board_{deviceName}_token");
            if (String.IsNullOrEmpty(token) == false)
            {
                var tokenArray = token.StringToByteArray();
                return tokenArray;
            }

            try
            {
                // First lets fetch it from OWCE servers.
                using (var handler = new HttpClientHandler())
                {
                    handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip;

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                        var response = await client.GetAsync($"https://{App.OWCEApiServer}/v1/handshake/{deviceName}");

                        // We only care if we were successful, otherwise fallback to FM.
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            var keyResponse = JsonSerializer.Deserialize<KeyResponse>(responseBody);

                            if (String.IsNullOrWhiteSpace(keyResponse.Key) == false)
                            {
                                await SecureStorage.SetAsync($"board_{deviceName}_key", apiKey);
                                await SecureStorage.SetAsync($"board_{deviceName}_token", keyResponse.Key);

                                var tokenArray = keyResponse.Key.StringToByteArray();
                                return tokenArray;
                            }
                        }

                        var statusResponse = await client.GetAsync($"https://{App.OWCEApiServer}/v1/status/handshake");
                        if (statusResponse.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            return null;
                        }

                        var handshakeStatusResponseBody = await statusResponse.Content.ReadAsStringAsync();
                        var handshakeStatusResponse = JsonSerializer.Deserialize<HandshakeStatusResponse>(handshakeStatusResponseBody);

                        if (handshakeStatusResponse.Online == false)
                        {
                            throw new Exceptions.HandshakeException(handshakeStatusResponse.Message, true);
                        }
                    }
                }

                // If we never found the key in OWCE servers lets fetch it from FM servers.
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
                            var activationResponse = JsonSerializer.Deserialize<FMActivationResponse>(responseBody);

                            if (String.IsNullOrWhiteSpace(activationResponse.Key))
                            {
                                throw new Exceptions.HandshakeException("Unable to fetch key. Please don't attempt this again. If you can, please report this to our Facebook page or discussion group.", true);
                            }

                            var keyRequest = new KeyRequest()
                            {
                                APIKey = apiKey,
                                BoardKey = activationResponse.Key,
                                DeviceName = deviceName,
                                OWType = owType,
                            };
                            var keyRequestJson = JsonSerializer.Serialize(keyRequest);
                            var httpContent = new StringContent(keyRequestJson, System.Text.Encoding.UTF8, "application/json");
                            response = await client.PutAsync($"https://{App.OWCEApiServer}/v1/handshake/{deviceName}", httpContent);


                            await SecureStorage.SetAsync($"board_{deviceName}_key", apiKey);
                            await SecureStorage.SetAsync($"board_{deviceName}_token", activationResponse.Key);

                            var tokenArray = activationResponse.Key.StringToByteArray();
                            return tokenArray;
                        }
                        else
                        {
                            throw new Exceptions.HandshakeException($"Unexpected response code ({response.StatusCode}). Please don't attempt this again. If you can, please report this to our Facebook page or discussion group.", true);
                        }
                    }
                }
            }
            catch (Exceptions.HandshakeException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                Debug.WriteLine($"ERROR: {err.Message}");

                SecureStorage.Remove($"board_{deviceName}_token");
                SecureStorage.Remove($"board_{deviceName}_key");
            }

            return null;
        }


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
                if (_isLogging)
                {
                    LogData(uuid, data);
                }
                else
                {
                    if (_initialEvents == null)
                    {
                        _initialEvents = new List<OWBoardEvent>();
                    }

                    _initialEvents.Add(new OWBoardEvent()
                    {
                        Uuid = uuid,
                        Data = ByteString.CopyFrom(data),
                        Timestamp = DateTime.UtcNow.Ticks,
                    });
                }
            }


            if (data.Length != 2)
                return;

            if (uuid == TemperatureUUID)
            {
                MotorTemperature = data[0];
                ControllerTemperature = data[1];

                return;
            }
            else if (uuid == BatteryTemperatureUUID)
            {
                if (_boardType == OWBoardType.V1 || _boardType == OWBoardType.Plus)
                {
                    BatteryTemperature = data[1];
                }
                else
                {
                    BatteryTemperature = data[0];
                }

                return;
            }
            else if (uuid == BatteryPercentUUID)
            {
                if (data[0] > 0)
                {
                    BatteryPercent = data[0];
                }
                else if (data[1] > 0)
                {
                    BatteryPercent = data[1];
                }

                return;
            }


            var value = BitConverter.ToUInt16(data, 0);


            switch (uuid)
            {
                case SerialNumberUUID:
                    SerialNumber = value;
                    break;
                case BatteryLow5UUID:
                    BatteryLow5 = value;
                    break;
                case BatteryLow20UUID:
                    BatteryLow20 = value;
                    break;
                case BatterySerialUUID:
                    BatterySerial = value;
                    break;
                case PitchUUID:
                    Pitch = 0.1f * (1800 - value);
                    break;
                case RollUUID:
                    Roll = 0.1f * (1800 - value);
                    break;
                case YawUUID:
                    Yaw = 0.1f * (1800 - value);
                    break;
                case TripOdometerUUID:
                    TripOdometer = value;
                    break;
                case RpmUUID:
                    RPM = value;
                    break;
                case RideModeUUID:
                    RideMode = value;
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
                    StatusError = value;
                    break;
                case FirmwareRevisionUUID:
                    FirmwareRevision = value;
                    break;
                case CurrentAmpsUUID:

                    var scaleFactor = _boardType switch {
                        OWBoardType.V1 => 0.0009f,
                        OWBoardType.Plus => 0.0018f,
                        OWBoardType.XR => 0.002f,
                        OWBoardType.Pint => 0.002f,
                        _ => throw new Exception("Unknown board type"),
                    };

                    /// https://en.wikipedia.org/wiki/Two's_complement
                    int ampsValue = (value > 32767) ? (int)value - 65536 : value;

                    CurrentAmps = (float)ampsValue * scaleFactor;
                    IsRegen = (CurrentAmps < 0);

                    break;
                case TripAmpHoursUUID:
                    if (BoardType == OWBoardType.V1)
                    {
                        TripAmpHours = (float)value * 0.00009f;
                    }
                    else
                    {
                        TripAmpHours = (float)value * 0.00018f;
                    }
                    break;
                case TripRegenAmpHoursUUID:
                    if (BoardType == OWBoardType.V1)
                    {
                        TripRegenAmpHours = (float)value * 0.00009f;
                    }
                    else
                    {
                        TripRegenAmpHours = (float)value * 0.00018f;
                    }
                    break;
                case BatteryVoltageUUID:
                    BatteryVoltage = 0.1f * value;
                    break;
                case SafetyHeadroomUUID:
                    SafetyHeadroom = value;
                    break;
                case HardwareRevisionUUID:
                    HardwareRevision = value;

                    if (value >= 1 && value <= 2999)
                    {
                        BoardType = OWBoardType.V1;
                        SimpleStopEnabled = null;
                    }
                    else if (value >= 3000 && value <= 3999)
                    {
                        BoardType = OWBoardType.Plus;
                        SimpleStopEnabled = null;
                    }
                    else if (value >= 4000 && value <= 4999)
                    {
                        BoardType = OWBoardType.XR;
                        SimpleStopEnabled = null;
                    }
                    else if (value >= 5000 && value <= 5999)
                    {
                        BoardType = OWBoardType.Pint;
                        if (SimpleStopEnabled == null)
                        {
                            SimpleStopEnabled = false;
                        }
                    }

                    if (HardwareRevision >= 4000)
                    {
                        BatteryCells.CellCount = 15;
                        BatteryCells.IgnoreCell(15);
                        OnPropertyChanged("BatteryCells");
                    }
                    else
                    {
                        BatteryCells.CellCount = 16;
                    }

                    break;
                case LifetimeOdometerUUID:
                    LifetimeOdometer = value;
                    break;
                case LifetimeAmpHoursUUID:
                    LifetimeAmpHours = value;
                    break;
                case BatteryCellsUUID:

                    // Different battery cell logic for XR 4210+ and Pint.
                    if (FirmwareRevision >= 4141)
                    {
                        var cellID = (uint)((value & 0xF000) >> 12);
                        var batteryVoltage = (value & 0x0FFF) * 0.0011f;
                        BatteryCells.SetCell(cellID, batteryVoltage, "F3");
                    }
                    else
                    {
                        var cellID = (uint)data[1];
                        var batteryVoltage = (float)data[0] * 0.02f;
                        BatteryCells.SetCell(cellID, batteryVoltage);
                    }

                    OnPropertyChanged("BatteryCells");

                    break;
                case LastErrorCodeUUID:

                    break;
                case UNKNOWN1UUID:
                    UNKNOWN1 = value;
                    break;
                case UNKNOWN2UUID:
                    UNKNOWN2 = value;
                    break;
                case UNKNOWN3UUID:
                    UNKNOWN3 = value;
                    break;
                case UNKNOWN4UUID:
                    UNKNOWN4 = value;
                    break;
            }
        }

        /*
        public async Task Disconnect()
        {
            _owble.BoardValueChanged -= OWBLE_BoardValueChanged;
            _keepHandshakeBackgroundRunning = false;
            await _owble.Disconnect();
            //await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(_device);
        }
        */
        //private long _currentRunStart = 0;
        //public long CurrentRunStart { get{ return _currentRunStart; } }


        public void StartLogging()
        {
            var filename = DateTime.Now.ToString("dd MMMM yyyy hh:mm:ss tt") + ".bin";

            // _currentRunStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            //_logDirectory = Path.Combine(FileSystem.CacheDirectory, _currentRunStart.ToString());
            var currentRunStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _currentRide = new Ride(filename);

            //_currentLogFile = Path.Combine(App.Current.LogsDirectory, filename);


            _isLogging = true;
            _events = new OWBoardEventList();
            if (_initialEvents != null)
            {
                _events.BoardEvents.AddRange(_initialEvents);
            }

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

        public string StopLogging()
        {
            _isLogging = false;
            _currentRide.EndTime = DateTime.Now;
            // TODO: Replace hud.
            //Hud.Show("Saving");

            /*
            await CrossGeolocator.Current.StopListeningAsync(); 
            CrossGeolocator.Current.PositionChanged -= PositionChanged;
            CrossGeolocator.Current.PositionError -= PositionError;
            */

            SaveEvents();

            //Hud.Dismiss();



            return String.Empty;
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
                var logPath = _currentRide.GetLogFilePath();
                using (FileStream fs = new FileStream(logPath, FileMode.Append, FileAccess.Write))
                {
                    foreach (var owBoardEvent in oldEvents.BoardEvents)
                    {
                        owBoardEvent.WriteDelimitedTo(fs);
                    }
                }
                //long currentRunEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                // var outputFile = Path.Combine(_logDirectory, $"{currentRunEnd}.dat");
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



        public async void ChangeRideMode(ushort rideMode)
        {
            _incomingRideMode = rideMode;
            byte[] rideModeBytes = BitConverter.GetBytes(rideMode);
            var result = await App.Current.OWBLE.WriteValue(OWBoard.RideModeUUID, rideModeBytes, true);
        }
    }
}
