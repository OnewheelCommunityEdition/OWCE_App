using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OWCE
{
    public class OWBaseBoard : Object, IEquatable<OWBaseBoard>, IEquatable<OWBoard>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected string _id = String.Empty;
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        protected string _name = String.Empty;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        protected bool _isAvailable = false;
        public bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }
            set
            {
                if (_isAvailable != value)
                {
                    _isAvailable = value;
                    OnPropertyChanged();
                }
            }
        }

        protected Object _nativePeripheral = null;
        public Object NativePeripheral
        {
            get { return _nativePeripheral; }
            set { if (_nativePeripheral != value) { _nativePeripheral = value; } }
        }

        // Value is in millimeters.
        protected float _wheelCircumference;
        public float WheelCircumference
        {
            get
            {
                return _wheelCircumference;
            }
            set
            {
                // Not checking against AlmostEqualTo, lets just update it regardless
                _wheelCircumference = value;
                OnPropertyChanged();
            }
        }

        /*
        // Value is in meters.
        protected float _tyreRadius;
        public float TyreRadius
        {
            get
            {
                return _tyreRadius;
            }
            set
            {
                // Not checking against AlmostEqualTo, lets just update it regardless
                _tyreRadius = value;
                OnPropertyChanged();
            }
        }
        */


        protected OWBoardType _boardType = OWBoardType.Unknown;
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

                    // TODO: Check for custom wheel circumference.

                    /*
                     * 11.5 inch wheel = 292.1 mm wheel
                     * Radius = 292.1 / 2 = 146.05
                     * Circumference = 2 * π * Radius
                     * Circumference = 917.66mm
                     * 
                     * 10.5 inch wheel = 266.7 mm wheel
                     * Radius = 266.7 / 2 = 133.35
                     * Circumference = 2 * π * Radius
                     * Circumference = 837.86mm
                     */
                    WheelCircumference = _boardType switch
                    {
                        OWBoardType.V1 => 917.66f,
                        OWBoardType.Plus => 917.66f,
                        OWBoardType.XR => 917.66f,
                        OWBoardType.Pint => 837.86f,
                        OWBoardType.PintX => 837.86f,
                        OWBoardType.GT => 917.66f,
                        _ => 0f,
                    };

                    OnPropertyChanged();
                    OnPropertyChanged("BoardModelStringShort");
                    OnPropertyChanged("BoardModelStringLong");
                    OnPropertyChanged("RideModeString");
                }
            }
        }

        public string BoardModelStringShort
        {
            get
            {
                return BoardType switch
                {
                    OWBoardType.V1 => "V1",
                    OWBoardType.Plus => "Plus",
                    OWBoardType.XR => "XR",
                    OWBoardType.Pint => "Pint",
                    OWBoardType.PintX => "Pint X",
                    OWBoardType.GT => "GT",
                    _ => String.Empty,
                };
            }
        }

        public string BoardModelStringLong
        {
            get
            {
                return BoardType switch
                {
                    OWBoardType.V1 => "Onewheel V1",
                    OWBoardType.Plus => "Onewheel+",
                    OWBoardType.XR => "Onewheel+ XR",
                    OWBoardType.Pint => "Onewheel Pint",
                    OWBoardType.PintX => "Onewheel Pint X",
                    OWBoardType.GT => "Onewheel GT",
                    _ => String.Empty,
                };
            }
        }

        public const float TwoPi = (2f * (float)Math.PI);
        public const float RadConvert = (TwoPi / 60f);


        public OWBaseBoard()
        {

        }

        public OWBaseBoard(string id, string name) : this()
        {
            _id = id;
            _name = name;
        }

        public OWBaseBoard(OWBaseBoard baseBoard)
        {
            ID = baseBoard.ID;
            Name = baseBoard.Name;
            IsAvailable = baseBoard.IsAvailable;
            NativePeripheral = baseBoard.NativePeripheral;
            WheelCircumference = baseBoard.WheelCircumference;
            BoardType = baseBoard.BoardType;
        }

        public bool Equals(OWBaseBoard otherBaseBoard)
        {
            return otherBaseBoard.ID == ID;
        }

        public bool Equals(OWBoard otherBoard)
        {
            return otherBoard.ID == ID;
        }

        public override bool Equals(object obj)
        {
            if (obj is OWBoard otherBoard)
            {
                return Equals(otherBoard);
            }
            else if (obj is OWBaseBoard otherBaseBoard)
            {
                return Equals(otherBaseBoard);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<(string, ushort)> GetAvailableRideModes()
        {
            if (_boardType == OWBoardType.V1)
            {
                return new List<(string, ushort)>()
                {
                    ("Classic", RideModes.V1_Classic),
                    ("Extreme", RideModes.V1_Extreme),
                    ("Elevated", RideModes.V1_Elevated),
                };
            }
            else if (_boardType == OWBoardType.Plus || _boardType == OWBoardType.XR)
            {
                return new List<(string, ushort)>()
                {
                    ("Sequoia", RideModes.PlusXR_Sequoia),
                    ("Cruz", RideModes.PlusXR_Cruz),
                    ("Mission", RideModes.PlusXR_Mission),
                    ("Elevated", RideModes.PlusXR_Elevated),
                    ("Delirium", RideModes.PlusXR_Delirium),
                    ("Custom", RideModes.PlusXR_Custom),
                };
            }
            else if (_boardType == OWBoardType.Pint || _boardType == OWBoardType.PintX)
            {
                return new List<(string, ushort)>()
                {
                    ("Redwood", RideModes.Pint_Redwood),
                    ("Pacific", RideModes.Pint_Pacific),
                    ("Elevated", RideModes.Pint_Elevated),
                    ("Skyline", RideModes.Pint_Skyline),
                    ("Custom", RideModes.Pint_Custom),
                };
            }
            else if (_boardType == OWBoardType.GT)
            {
                return new List<(string, ushort)>()
                {
                    ("Bay", RideModes.GT_Bay),
                    ("Roam", RideModes.GT_Roam),
                    ("Flow", RideModes.GT_Flow),
                    ("Highline", RideModes.GT_HighLine),
                    ("Elevated", RideModes.GT_Elevated),
                    ("Apex", RideModes.GT_Apex),
                    ("Custom", RideModes.GT_Custom),
                };
            }

            return new List<(string, ushort)>();
        }
    }
}
