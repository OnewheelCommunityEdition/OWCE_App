using System;
namespace OWCE.Models
{
    public class RideModeBoardDetail : BaseBoardDetail
    {
        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                    OnPropertyChanged("RideModeString");
                }
            }
        }

        private OWBoardType _boardType;
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
                    OnPropertyChanged();
                    OnPropertyChanged("RideModeString");
                }
            }
        }

        public string RideModeString
        {
            get
            {
                if (_boardType == OWBoardType.V1)
                {
                    return _value switch
                    {
                        1 => "Classic",
                        2 => "Extreme",
                        3 => "Elevated",
                        _ => "Unknown",
                    };
                }
                else if (_boardType == OWBoardType.Plus || _boardType == OWBoardType.XR)
                {
                    return _value switch
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
                    return _value switch
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

        public RideModeBoardDetail(string name) : base(name)
        {
        }
    }
}
