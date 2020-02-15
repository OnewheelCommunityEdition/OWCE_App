using System;
using System.ComponentModel;

namespace OWCE
{
    public class BoardDetail : Object, INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public BoardDetail()
        {

        }

        public BoardDetail(string name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TemperatureBoardDetail : BoardDetail
    {

        public TemperatureBoardDetail(string name) : base(name)
        {

        }
    }

    public class BatteryCellsBoardDetail : BoardDetail
    {
        public string BatteryCell0 { get; set; } = "-";
        public string BatteryCell1 { get; set; } = "-";
        public string BatteryCell2 { get; set; } = "-";
        public string BatteryCell3 { get; set; } = "-";
        public string BatteryCell4 { get; set; } = "-";
        public string BatteryCell5 { get; set; } = "-";
        public string BatteryCell6 { get; set; } = "-";
        public string BatteryCell7 { get; set; } = "-";
        public string BatteryCell8 { get; set; } = "-";
        public string BatteryCell9 { get; set; } = "-";
        public string BatteryCell10 { get; set; } = "-";
        public string BatteryCell11 { get; set; } = "-";
        public string BatteryCell12 { get; set; } = "-";
        public string BatteryCell13 { get; set; } = "-";
        public string BatteryCell14 { get; set; } = "-";
        public string BatteryCell15 { get; set; } = "-";
    }


    public class VoltageBoardDetail : BoardDetail
    {
        private float _value;
        public new float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value.AlmostEqualTo(value))
                    return;

                _value = value;
                OnPropertyChanged();
            }
        }

        public VoltageBoardDetail(string name) : base(name)
        {

        }
    }

    public class SpeedBoardDetail : BoardDetail
    {

    }

    public class AngleBoardDetail : BoardDetail
    {
        private float _value;
        public new float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value.AlmostEqualTo(value))
                    return;

                _value = value;
                OnPropertyChanged();
            }
        }

        public AngleBoardDetail(string name) : base(name)
        {

        }
    }
}
