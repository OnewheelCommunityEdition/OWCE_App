using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using OWCE.Protobuf;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Essentials;

namespace OWCE
{
    public enum OWBoardType
    {
        Unknown,
        V1,
        Plus,
        XR
    };

    public class OWBoard : object, IEquatable<OWBoard>, INotifyPropertyChanged
    {
        public const string ServiceUUID = "E659F300-EA98-11E3-AC10-0800200C9A66";
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



        private string _id = String.Empty;
        [SQLite.PrimaryKey]
        public string ID
        {
            get { return _id; }
            set { if (_id != value) { _id = value; OnPropertyChanged(); } }
        }

        private string _name = String.Empty;
        public string Name
        {
            get { return _name; }
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }


        private bool _isAvailable = false;
        public bool IsAvailable
        {
            get { return _isAvailable; }
            set { if (_isAvailable != value) { _isAvailable = value; OnPropertyChanged(); } }
        }


        private OWBoardType _boardType = OWBoardType.Unknown;
        public OWBoardType BoardType
        {
            get { return _boardType; }
            set { if (_boardType != value) { _boardType = value; OnPropertyChanged(); } }
        }

        private IDevice _device = null;
        [SQLite.Ignore]
        public IDevice Device
        {
            get { return _device; }
            set { if (_device != value) { _device = value; OnPropertyChanged(); } }
        }


        public string BoardIcon
        {
            get
            {
                switch (BoardType)
                {
                    case OWBoardType.V1:
                        return "ow_icon_v1.png";
                    case OWBoardType.Plus:
                        return "ow_icon_plus.png";
                    case OWBoardType.XR:
                        return "ow_icon_xr.png";
                }
                return "ow_icon.png";
            }
        }

        public int MaxRecommendedSpeed
        {
            get
            {
                var isMetric = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);

                if (isMetric)
                {
                    switch (RideMode)
                    {
                        // v1
                        case 1: // Classic
                            return 19;
                        case 2: // Extreme
                            return 24; // 15
                        case 3: // Extreme
                            return 24; // 15

                        // Plus/XR
                        case 4: // Sequoia
                            return 19; // 12
                        case 5: // Cruz
                            return 24; //15
                        case 6: // Mission
                            return 30; // 19
                        case 7: // Elevated
                            return 30; // 19
                        case 8: // Delirium
                            return 32; // 20
                    }

                    // New ride mode?
                    return 30;
                }
                else
                {
                    switch (RideMode)
                    {
                        // v1
                        case 1: // Classic
                            return 12;
                        case 2: // Extreme
                            return 15;
                        case 3: // Extreme
                            return 15;

                        // Plus/XR
                        case 4: // Sequoia
                            return 12;
                        case 5: // Cruz
                            return 15;
                        case 6: // Mission
                            return 19;
                        case 7: // Elevated
                            return 19;
                        case 8: // Delirium
                            return 20;
                    }

                    // New ride mode?
                    return 19;
                }
            }
        }

        public int MaxDisplaySpeed
        {
            get
            {
                var speedDemon = Preferences.Get("speed_demon", false);
                if (speedDemon == false)
                {
                    return MaxRecommendedSpeed;
                }

                var isMetric = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);

                if (isMetric)
                {
                    return 50;
                }
                else
                {
                    return 30;
                }
            }
        }



        private int _serialNumber = 0;
        public int SerialNumber
        {
            get { return _serialNumber; }
            set { if (_serialNumber != value) { _serialNumber = value; OnPropertyChanged(); } }
        }

        private int _rideMode = 0;
        public int RideMode
        {
            get { return _rideMode; }
            set
            {
                if (_rideMode != value)
                {
                    _rideMode = value;
                    OnPropertyChanged();

                    // Update these values because their display is based on ride mode.
                    OnPropertyChanged("MaxDisplaySpeed");
                    OnPropertyChanged("MaxRecommendedSpeed");
                }
            }
        }

        private int _batteryPercent = 0;
        public int BatteryPercent
        {
            get { return _batteryPercent; }
            set { if (_batteryPercent != value) { _batteryPercent = value; OnPropertyChanged(); } }
        }

        private int _batteryLow5 = 0;
        public int BatteryLow5
        {
            get { return _batteryLow5; }
            set { if (_batteryLow5 != value) { _batteryLow5 = value; OnPropertyChanged(); } }
        }

        private int _batteryLow20 = 0;
        public int BatteryLow20
        {
            get { return _batteryLow20; }
            set { if (_batteryLow20 != value) { _batteryLow20 = value; OnPropertyChanged(); } }
        }

        private int _batterySerial = 0;
        public int BatterySerial
        {
            get { return _batterySerial; }
            set { if (_batterySerial != value) { _batterySerial = value; OnPropertyChanged(); } }
        }

        private float _pitch = 0;
        public float Pitch
        {
            get { return _pitch; }
            set { if (_pitch != value) { _pitch = value; OnPropertyChanged(); } }
        }

        private float _yaw = 0;
        public float Yaw
        {
            get { return _yaw; }
            set { if (_yaw != value) { _yaw = value; OnPropertyChanged(); } }
        }

        private float _tripOdometer = 0;
        public float TripOdometer
        {
            get { return _tripOdometer; }
            set { if (_tripOdometer != value) { _tripOdometer = value; OnPropertyChanged(); } }
        }

        private float _roll = 0;
        public float Roll
        {
            get { return _roll; }
            set { if (_roll != value) { _roll = value; OnPropertyChanged(); } }
        }

        private int _rpm = 0;
        public int RPM
        {
            get { return _rpm; }
            set
            {
                if (_rpm != value)
                {
                    _rpm = value;
                    OnPropertyChanged();

                    var circumference = 910f; //mm
                    var radius = circumference / (2f * (float)Math.PI) / 1000f; // In meters
                    var radPerSecond = (((float)Math.PI * 2f) / 60f) * _rpm;
                    var speedInMetersPerSecond = radius * radPerSecond;

                    var isMetric = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);
                    if (isMetric)
                    {
                        var speedInKilometersPerHour = speedInMetersPerSecond * 3.6f;
                        Speed = speedInKilometersPerHour;
                    }
                    else
                    {
                        var speedInMilesPerHour = speedInMetersPerSecond * 2.23694f;
                        Speed = speedInMilesPerHour;
                    }

                    /*
                    Console.WriteLine("RPM: " + _rpm);
                    Console.WriteLine("m/s: " + speedInMetersPerSecond);
                    Console.WriteLine("km/h: " + speedInKilometersPerHour);
                    */
                    // TODO: Metric check
                    // TODO: Convert RPM to speed.
                    //Speed = _rpm;
                }
            }
        }

        private float _speed = 0;
        public float Speed
        {
            get { return _speed; }
            set { if (_speed != value) { _speed = value; OnPropertyChanged(); } }
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

        private int _statusError = 0;
        public int StatusError
        {
            get { return _statusError; }
            set { if (_statusError != value) { _statusError = value; OnPropertyChanged(); } }
        }

        private float _temperature = 0;
        public float Temperature
        {
            get { return _temperature; }
            set { if (_temperature != value) { _temperature = value; OnPropertyChanged(); } }
        }

        private int _firmwareRevision = 0;
        public int FirmwareRevision
        {
            get { return _firmwareRevision; }
            set { if (_firmwareRevision != value) { _firmwareRevision = value; OnPropertyChanged(); } }
        }

        private float _currentAmps = 0;
        public float CurrentAmps
        {
            get { return _currentAmps; }
            set { if (_currentAmps != value) { _currentAmps = value; OnPropertyChanged(); } }
        }

        private float _tripAmpHours = 0;
        public float TripAmpHours
        {
            get { return _tripAmpHours; }
            set { if (_tripAmpHours != value) { _tripAmpHours = value; OnPropertyChanged(); } }
        }

        private float _tripRegenAmpHours = 0;
        public float TripRegenAmpHours
        {
            get { return _tripRegenAmpHours; }
            set { if (_tripRegenAmpHours != value) { _tripRegenAmpHours = value; OnPropertyChanged(); } }
        }

        private float _batteryTemperature = 0;
        public float BatteryTemperature
        {
            get { return _batteryTemperature; }
            set { if (_batteryTemperature != value) { _batteryTemperature = value; OnPropertyChanged(); } }
        }

        private float _batteryVoltage = 0;
        public float BatteryVoltage
        {
            get { return _batteryVoltage; }
            set { if (_batteryVoltage != value) { _batteryVoltage = value; OnPropertyChanged(); } }
        }

        private int _safetyHeadroom = 0;
        public int SafetyHeadroom
        {
            get { return _safetyHeadroom; }
            set { if (_safetyHeadroom != value) { _safetyHeadroom = value; OnPropertyChanged(); } }
        }

        private int _hardwareRevision = 0;
        public int HardwareRevision
        {
            get { return _hardwareRevision; }
            set
            {
                if (_hardwareRevision != value)
                {
                    _hardwareRevision = value;
                    OnPropertyChanged();

                    if (_hardwareRevision >= 1 && _hardwareRevision <= 2999)
                    {
                        BoardType = OWBoardType.V1;
                    }
                    else if (_hardwareRevision >= 3000 && _hardwareRevision <= 3999)
                    {
                        BoardType = OWBoardType.Plus;
                    }
                    else if (_hardwareRevision >= 4000 && _hardwareRevision <= 4999)
                    {
                        BoardType = OWBoardType.XR;
                    }
                }
            }
        }

        private float _lifetimeOdometer = 0;
        public float LifetimeOdometer
        {
            get { return _lifetimeOdometer; }
            set { if (_lifetimeOdometer != value) { _lifetimeOdometer = value; OnPropertyChanged(); } }
        }

        private float _lifetimeAmpHours = 0;
        public float LifetimeAmpHours
        {
            get { return _lifetimeAmpHours; }
            set { if (_lifetimeAmpHours != value) { _lifetimeAmpHours = value; OnPropertyChanged(); } }
        }

        /*
        private float _batteryCells = 0;
        public float BatteryCells
        {
            get { return _batteryCells; }
            set { if (_batteryCells != value) { _batteryCells = value; OnPropertyChanged(); } }
        }
        */

        private float _lastErrorCode = 0;
        public float LastErrorCode
        {
            get { return _lastErrorCode; }
            set { if (_lastErrorCode != value) { _lastErrorCode = value; OnPropertyChanged(); } }
        }

        private float _UNKNOWN1 = 0;
        public float UNKNOWN1
        {
            get { return _UNKNOWN1; }
            set { if (_UNKNOWN1 != value) { _UNKNOWN1 = value; OnPropertyChanged(); } }
        }

        private float _UNKNOWN2 = 0;
        public float UNKNOWN2
        {
            get { return _UNKNOWN2; }
            set { if (_UNKNOWN2 != value) { _UNKNOWN2 = value; OnPropertyChanged(); } }
        }

        private float _UNKNOWN3 = 0;
        public float UNKNOWN3
        {
            get { return _UNKNOWN3; }
            set { if (_UNKNOWN3 != value) { _UNKNOWN3 = value; OnPropertyChanged(); } }
        }

        private float _UNKNOWN4 = 0;
        public float UNKNOWN4
        {
            get { return _UNKNOWN4; }
            set { if (_UNKNOWN4 != value) { _UNKNOWN4 = value; OnPropertyChanged(); } }
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

        public OWBoard()
        {

        }

        public bool Equals(OWBoard otherBoard)
        {
            return otherBoard.ID == ID;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is OWBoard otherBoard)
            {
                return Equals(otherBoard);
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        IService _service = null;
        IList<ICharacteristic> _characteristics = null;

        private void RSSIMonitor()
        {
            Task.Factory.StartNew(async () =>
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
                await Task.Delay(50);

                if (_device.State == Plugin.BLE.Abstractions.DeviceState.Connected)
                {
                    // I hope this won't crash the app.
                    RSSIMonitor();
                }
            });
        }

        internal async void SubscribeToBLE()
        {
#if DEBUG
            if (_device == null)
                return;
#endif
            RSSIMonitor();




            List<string> characteristicsToSubscribeTo = new List<string>()
            {
                SerialNumberUUID,
                BatteryPercentUUID,
                BatteryLow5UUID,
                BatteryLow20UUID,
                BatterySerialUUID,
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
                FirmwareRevisionUUID,
                CurrentAmpsUUID,
                TripAmpHoursUUID,
                TripRegenAmpHoursUUID,
                BatteryTemperatureUUID,
                BatteryVoltageUUID,
                SafetyHeadroomUUID,
                HardwareRevisionUUID,
                LifetimeOdometerUUID,
                LifetimeAmpHoursUUID,
                BatteryCellsUUID,
                LastErrorCodeUUID,
                //SerialRead,
                //SerialWrite,
                UNKNOWN1UUID,
                UNKNOWN2UUID,
                UNKNOWN3UUID,
                UNKNOWN4UUID,
            };



            _service = await _device.GetServiceAsync(new Guid(ServiceUUID.ToLower()));
            _characteristics = await _service.GetCharacteristicsAsync();

            var readTasks = new Dictionary<string, Task<byte[]>>();
            //var readTask = new List<Task<byte[]>>();
            foreach (var characteristic in _characteristics)
            {
                var uuid = characteristic.Uuid.ToUpper();
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

        private void SetValue(string uuid, byte[] data, bool initialData = false)
        {
            // If our system is little endian, reverse the array.
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            var value = BitConverter.ToUInt16(data, 0);


            if (initialData)
            {
                _initialEvents.Add(new OWBoardEvent()
                {
                    Uuid = uuid,
                    Data = ByteString.CopyFrom(data),
                    Timestamp = DateTime.UtcNow.Ticks,
                });
            }

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
                    SerialNumber = value;
                    break;
                case BatteryPercentUUID:
                    BatteryPercent = value;
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
                case TemperatureUUID:
                    Temperature = 0.1f * value;
                    break;
                case FirmwareRevisionUUID:
                    FirmwareRevision = value;
                    break;
                case CurrentAmpsUUID:
                    CurrentAmps = 0.1f * value;
                    break;
                case TripAmpHoursUUID:
                    TripAmpHours = 0.1f * value;
                    break;
                case TripRegenAmpHoursUUID:
                    TripRegenAmpHours = 0.1f * value;
                    break;
                case BatteryTemperatureUUID:
                    BatteryTemperature = 0.1f * value;
                    break;
                case BatteryVoltageUUID:
                    BatteryVoltage = 0.1f * value;
                    break;
                case SafetyHeadroomUUID:
                    SafetyHeadroom = value;
                    break;
                case HardwareRevisionUUID:
                    HardwareRevision = value;
                    break;
                case LifetimeOdometerUUID:
                    LifetimeOdometer = (float)value;
                    break;
                case LifetimeAmpHoursUUID:
                    LifetimeAmpHours = value;
                    break;
                case BatteryCellsUUID:

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

        void Characteristic_ValueUpdated(object sender, Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs e)
        {
            SetValue(e.Characteristic.Uuid.ToUpper(), e.Characteristic.Value);
        }

        public async Task Disconnect()
        {
            await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(_device);
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



            if (CrossGeolocator.Current.IsGeolocationAvailable)
            {
                CrossGeolocator.Current.DesiredAccuracy = 1;

                await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true);

                CrossGeolocator.Current.PositionChanged += PositionChanged;
                CrossGeolocator.Current.PositionError += PositionError;

            }
        }

        public async Task<string> StopLogging()
        {
            _isLogging = false;
            _currentRide.EndTime = DateTime.Now;


            Hud.Show("Compressing data");
            await CrossGeolocator.Current.StopListeningAsync(); ;

            CrossGeolocator.Current.PositionChanged -= PositionChanged;
            CrossGeolocator.Current.PositionError -= PositionError;

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
    }

}
