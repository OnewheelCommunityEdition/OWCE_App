using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OWCE.Models
{
    public class BatteryCells : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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


        private Dictionary<uint, float> _cells = new Dictionary<uint, float>();
        private List<uint> _ignoredCells = new List<uint>();

        private int _cellCount = 0;
        public int CellCount
        {
            get { return _cellCount; }
            set
            {
                if (_cellCount != value)
                {
                    _cellCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private float _lowestCellVoltage;
        public float LowestCellVoltage
        {
            get
            {
                return _lowestCellVoltage;
            }
            set
            {
                if (_lowestCellVoltage != value)
                {
                    _lowestCellVoltage = value;
                    OnPropertyChanged();
                }
            }
        }

        private float _highestCellVoltage;
        public float HighestCellVoltage
        {
            get
            {
                return _highestCellVoltage;
            }
            set
            {
                if (_highestCellVoltage != value)
                {
                    _highestCellVoltage = value;
                    OnPropertyChanged();
                }
            }
        }

        private float _averageCellVoltage;
        public float AverageCellVoltage
        {
            get
            {
                return _averageCellVoltage;
            }
            set
            {
                if (_averageCellVoltage != value)
                {
                    _averageCellVoltage = value;
                    OnPropertyChanged();
                }
            }
        }

        public BatteryCells()
        {

        }

        public void IgnoreCell(uint cellID)
        {
            if (_ignoredCells.Contains(cellID) == false)
            {
                _ignoredCells.Add(cellID);
            }

            SetCellVoltageString(cellID, String.Empty);
        }

        public void SetCell(uint cellID, float voltage, string format = "F2")
        {
            if (_cells.ContainsKey(cellID) == false || _cells[cellID] != voltage)
            {
                _cells[cellID] = voltage;

                // TODO: How to handle this by reducing allocations. Spans?
                var cells = _cells.Values.Where(v => v > 0);
                LowestCellVoltage = cells.Min();
                HighestCellVoltage = cells.Max();
                AverageCellVoltage = cells.Average();
                cells = null;
            }

            if (_ignoredCells.Contains(cellID))
            {
                return;
            }

            var voltageString = voltage.ToString(format) + "V";
            SetCellVoltageString(cellID, voltageString);
        }

        public float GetCell(uint id)
        {
            if (_cells.ContainsKey(id))
            {
                return _cells[id];
            }

            return 0;
        }

        private void SetCellVoltageString(uint cellID, string voltageString)
        {
            switch (cellID)
            {
                case 0:
                    if (BatteryCell0 != voltageString)
                    {
                        BatteryCell0 = voltageString;
                        OnPropertyChanged("BatteryCell0");
                    }
                    break;
                case 1:
                    if (BatteryCell1 != voltageString)
                    {
                        BatteryCell1 = voltageString;
                        OnPropertyChanged("BatteryCell1");
                    }
                    break;
                case 2:
                    if (BatteryCell2 != voltageString)
                    {
                        BatteryCell2 = voltageString;
                        OnPropertyChanged("BatteryCell2");
                    }
                    break;
                case 3:
                    if (BatteryCell3 != voltageString)
                    {
                        BatteryCell3 = voltageString;
                        OnPropertyChanged("BatteryCell3");
                    }
                    break;
                case 4:
                    if (BatteryCell4 != voltageString)
                    {
                        BatteryCell4 = voltageString;
                        OnPropertyChanged("BatteryCell4");
                    }
                    break;
                case 5:
                    if (BatteryCell5 != voltageString)
                    {
                        BatteryCell5 = voltageString;
                        OnPropertyChanged("BatteryCell5");
                    }
                    break;
                case 6:
                    if (BatteryCell6 != voltageString)
                    {
                        BatteryCell6 = voltageString;
                        OnPropertyChanged("BatteryCell6");
                    }
                    break;
                case 7:
                    if (BatteryCell7 != voltageString)
                    {
                        BatteryCell7 = voltageString;
                        OnPropertyChanged("BatteryCell7");
                    }
                    break;
                case 8:
                    if (BatteryCell8 != voltageString)
                    {
                        BatteryCell8 = voltageString;
                        OnPropertyChanged("BatteryCell8");
                    }
                    break;
                case 9:
                    if (BatteryCell9 != voltageString)
                    {
                        BatteryCell9 = voltageString;
                        OnPropertyChanged("BatteryCell9");
                    }
                    break;
                case 10:
                    if (BatteryCell10 != voltageString)
                    {
                        BatteryCell10 = voltageString;
                        OnPropertyChanged("BatteryCell10");
                    }
                    break;
                case 11:
                    if (BatteryCell11 != voltageString)
                    {
                        BatteryCell11 = voltageString;
                        OnPropertyChanged("BatteryCell11");
                    }
                    break;
                case 12:
                    if (BatteryCell12 != voltageString)
                    {
                        BatteryCell12 = voltageString;
                        OnPropertyChanged("BatteryCell12");
                    }
                    break;
                case 13:
                    if (BatteryCell13 != voltageString)
                    {
                        BatteryCell13 = voltageString;
                        OnPropertyChanged("BatteryCell13");
                    }
                    break;
                case 14:
                    if (BatteryCell14 != voltageString)
                    {
                        BatteryCell14 = voltageString;
                        OnPropertyChanged("BatteryCell14");
                    }
                    break;
                case 15:
                    if (BatteryCell15 != voltageString)
                    {
                        BatteryCell15 = voltageString;
                        OnPropertyChanged("BatteryCell15");
                    }
                    break;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
