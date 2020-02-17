using System;
using System.Collections.Generic;

namespace OWCE.Models
{
    public class BatteryCellsBoardDetail : BaseBoardDetail
    {
        public string BatteryCell0 { get; private set; } = "-";
        public string BatteryCell1 { get; private set; } = "-";
        public string BatteryCell2 { get; private set; } = "-";
        public string BatteryCell3 { get; private set; } = "-";
        public string BatteryCell4 { get; private set; } = "-";
        public string BatteryCell5 { get; private set; } = "-";
        public string BatteryCell6 { get; private set; } = "-";
        public string BatteryCell7 { get; private set; } = "-";
        public string BatteryCell8 { get; private set; } = "-";
        public string BatteryCell9 { get; private set; } = "-";
        public string BatteryCell10 { get; private set; } = "-";
        public string BatteryCell11 { get; private set; } = "-";
        public string BatteryCell12 { get; private set; } = "-";
        public string BatteryCell13 { get; private set; } = "-";
        public string BatteryCell14 { get; private set; } = "-";
        public string BatteryCell15 { get; private set; } = "-";


        private Dictionary<uint, uint> _cells = new Dictionary<uint, uint>();

        public BatteryCellsBoardDetail(string name) : base(name)
        {

        }

        public void SetCell(uint cellID, uint? value)
        {
            _cells[cellID] = value ?? 0;

            var voltageString = String.Empty;
            if (value != null)
            {
                voltageString = (0.02f * value ?? 0).ToString("F2") + "V";
            }

            switch (cellID)
            {
                case 0:
                    BatteryCell0 = voltageString;
                    OnPropertyChanged("BatteryCell0");
                    break;
                case 1:
                    BatteryCell1 = voltageString;
                    OnPropertyChanged("BatteryCell1");
                    break;
                case 2:
                    BatteryCell2 = voltageString;
                    OnPropertyChanged("BatteryCell2");
                    break;
                case 3:
                    BatteryCell3 = voltageString;
                    OnPropertyChanged("BatteryCell3");
                    break;
                case 4:
                    BatteryCell4 = voltageString;
                    OnPropertyChanged("BatteryCell4");
                    break;
                case 5:
                    BatteryCell5 = voltageString;
                    OnPropertyChanged("BatteryCell5");
                    break;
                case 6:
                    BatteryCell6 = voltageString;
                    OnPropertyChanged("BatteryCell6");
                    break;
                case 7:
                    BatteryCell7 = voltageString;
                    OnPropertyChanged("BatteryCell7");
                    break;
                case 8:
                    BatteryCell8 = voltageString;
                    OnPropertyChanged("BatteryCell8");
                    break;
                case 9:
                    BatteryCell9 = voltageString;
                    OnPropertyChanged("BatteryCell9");
                    break;
                case 10:
                    BatteryCell10 = voltageString;
                    OnPropertyChanged("BatteryCell10");
                    break;
                case 11:
                    BatteryCell11 = voltageString;
                    OnPropertyChanged("BatteryCell11");
                    break;
                case 12:
                    BatteryCell12 = voltageString;
                    OnPropertyChanged("BatteryCell12");
                    break;
                case 13:
                    BatteryCell13 = voltageString;
                    OnPropertyChanged("BatteryCell13");
                    break;
                case 14:
                    BatteryCell14 = voltageString;
                    OnPropertyChanged("BatteryCell14");
                    break;
                case 15:
                    BatteryCell15 = voltageString;
                    OnPropertyChanged("BatteryCell15");
                    break;
            }
        }

        public uint GetCell(uint id)
        {
            if (_cells.ContainsKey(id))
            {
                return _cells[id];
            }

            return 0;
        }
    }
}
