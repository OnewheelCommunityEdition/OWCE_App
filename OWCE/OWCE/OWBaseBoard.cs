using System;
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

        // Value is in milimeters.
        protected float _tyreCircumference;
        public float TyreCircumference
        {
            get
            {
                return _tyreCircumference;
            }
            set
            {
                // Not checking against AlmostEqualTo, lets just update it regardless
                _tyreCircumference = value;
                OnPropertyChanged();

                TyreRadius = _tyreCircumference / TwoPi / 1000f;
            }
        }

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

        public const float TwoPi = (2f * (float)Math.PI);
        public const float RadConvert = (TwoPi / 60f);



        public OWBaseBoard()
        {
            TyreCircumference = 910f;
        }

        public OWBaseBoard(string id, string name) : base()
        {
            _id = id;
            _name = name;
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
    }
}
