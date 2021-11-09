using System;

using Xamarin.Forms;
using OWCE.Views;
using OWCE.Models;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.Generic;

namespace OWCE.Views
{
    public class BatteryCellsView : ContentView
    {
        /*
        public static readonly BindableProperty CellCountProperty = BindableProperty.Create(nameof(CellCount), typeof(int), typeof(BatteryCellsView), default(int));
        public int CellCount
        {
            get => (int)GetValue(CellCountProperty);
            set => SetValue(CellCountProperty, value);
        }

        public static readonly BindableProperty BatteryCellsProperty = BindableProperty.Create(nameof(BatteryCells), typeof(BatteryCells), typeof(BatteryCellsView), default);       
        public BatteryCells BatteryCells
        {
            get => (BatteryCells)GetValue(BatteryCellsProperty);
            set => SetValue(BatteryCellsProperty, value);
        }
        */

        Dictionary<uint, Label> _cellLables = new Dictionary<uint, Label>();

        public BatteryCellsView()
        {
            Content = new Grid()
            {
                ColumnSpacing = 0,
                RowSpacing = 0,
            };
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (BindingContextProperty.PropertyName.Equals(propertyName))
            {
                // TODO: Unsubscribe from this.
                if (BindingContext is BatteryCells batteryCells)
                {
                    batteryCells.PropertyChanged += BatteryCells_PropertyChanged;
                    SetupGrid();
                }
            }
        }

        private void SetupGrid()
        {
            if (Content is Grid grid && BindingContext is BatteryCells batteryCells)
            {
                _cellLables.Clear();
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
                grid.ColumnDefinitions.Clear();

                int columns = 0;
                int rows = 0;
                if (batteryCells.CellCount == 15)
                {
                    rows = 3;
                    columns = 5;
                }
                else if (batteryCells.CellCount == 16)
                {
                    rows = 4;
                    columns = 4;
                }
                else
                {
                    return;
                }

                // Setup row definitions.
                for (var row = 0; row < rows; ++row)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(45) });
                }

                // Setup column definitions (not required, its * by default)
                for (var column = 0; column < columns; ++column)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                }

                /*
                var minVoltage = 2.8f;
                var maxVoltage = 4.2f;
                var voltageRange = maxVoltage - minVoltage; //1.4f
                var voltagePerIndex = voltageRange / batteryCells.CellCount;
                */
                var color = Color.Magenta;

                for (var row = 0; row < rows; ++row)
                {
                    for (var column = 0; column < columns; ++column)
                    {
                        var cellIndex = column + (columns * row);

                        //var voltage = minVoltage + (voltagePerIndex * cellIndex);
                        var label = new Label()
                        {
                            TextColor = Color.White,
                            Text = "-",
                            FontSize = 24,
                            FontFamily = "SairaExtraCondensed-Bold",
                            BackgroundColor = Color.Magenta,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                        };


                        Grid.SetRow(label, row);
                        Grid.SetColumn(label, column);

                        grid.Children.Add(label);

                        //var cellKey = $"BatteryCell{cellIndex}";
                        _cellLables.Add((uint)cellIndex, label);
                    }
                }

            }
        }



        private void BatteryCells_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.StartsWith("BatteryCell"))
            {
                var labelSpan = e.PropertyName.AsSpan();

                // 11 = "BatteryCell".Length
                var cellIDSpan = labelSpan.Slice(11);

                if (UInt32.TryParse(cellIDSpan, out uint cellID))
                {
                    UpdateCell(cellID);
                }
            }
            else if (e.PropertyName.StartsWith("CellCount"))
            {
                SetupGrid();
            }
        }

        void UpdateCell(uint cellID)
        {
            if (BindingContext is BatteryCells batteryCells && _cellLables.ContainsKey(cellID))
            {
                var value = batteryCells.GetCell(cellID);
                var label = _cellLables[cellID];
                var color = GetColor(value);

                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    label.Text = value.ToString("F2");
                    label.BackgroundColor = color;
                });
            }
        }

        Color GetColor(float voltage)
        {
            var minVoltage = 2.8f;
            var maxVoltage = 4.2f;

            if (voltage < minVoltage)
            {
                voltage = minVoltage;
            }
            else if (voltage > maxVoltage)
            {
                voltage = maxVoltage;
            }

            var voltageRange = maxVoltage - minVoltage; //1.4f
            var voltageOffset = voltage - minVoltage; // 0.46
            var voltagePercent = 1f - (voltageOffset / voltageRange);
            var colorPercent = (voltagePercent * 0.788235294118);

            //System.Diagnostics.Debug.WriteLine($"{voltage:F2} - {colorPercent:F2}");
            return new Color(1f, colorPercent, 1f);


            /*
            var color = voltage switch
            {
                var v when v <= 2.8 => new Color(1f, 201f / 255f, 1f),
                var v when v > 2.8 => new Color(1f, 201f / 255f, 1f),

            };

            */

            return Color.Magenta;



        }
    }
}

