using OWCE.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.RangeSlider.Forms;
using RangeSlider = Xamarin.RangeSlider.Forms.RangeSlider;

namespace OWCE.Views
{
    public class RideModeButton : INotifyPropertyChanged
    {
        protected string _name;
        public string Name
        {
            get { return _name; }
            set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        protected ushort _value;
        public ushort Value
        {
            get { return _value; }
            set { if (_value != value) { _value = value; OnPropertyChanged(); } }
        }

        protected int _rideMode;
        public int RideMode
        {
            get { return _rideMode; }
            set { if (_rideMode != value) { _rideMode = value; OnPropertyChanged(); } }
        }

        protected bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class RideModeView : ContentView
    {
        public static readonly BindableProperty BoardTypeProperty = BindableProperty.Create(
            "BoardType",
            typeof(OWBoardType),
            typeof(RideModeView));

        TurnCompensationConverter turnCompensationConverter = new TurnCompensationConverter();
        AggressivenessConverter aggressivenessConverter = new AggressivenessConverter();
        AngleOffsetConverter angleOffsetConverter = new AngleOffsetConverter();

        public OWBoardType BoardType
        {
            get { return (OWBoardType)GetValue(BoardTypeProperty); }
            set
            {
                SetValue(BoardTypeProperty, value);
            }
        }

        public static readonly BindableProperty SimpleStopEnabledProperty = BindableProperty.Create(
            "SimpleStopEnabled",
            typeof(bool?),
            typeof(RideModeView));

        public bool? SimpleStopEnabled
        {
            get { return (bool?)GetValue(SimpleStopEnabledProperty); }
            set
            {
                SetValue(SimpleStopEnabledProperty, value);
            }
        }

        ObservableCollection<RideModeButton> _rideModesList = new ObservableCollection<RideModeButton>();

        public RideModeView()
        {
            InitializeComponent();
        }

        void ExpanderView_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Expander.IsExpandedProperty.PropertyName.Equals(e.PropertyName))
            {
                if (ExpanderView.IsExpanded)
                {
                    ExpanderArrow.RotateTo(180, ExpanderView.ExpandAnimationLength, ExpanderView.ExpandAnimationEasing);
                }
                else
                {
                    ExpanderArrow.RotateTo(0, ExpanderView.CollapseAnimationLength, ExpanderView.CollapseAnimationEasing);
                }
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is OWBoard board)
            {
                _availableRideModes = board.GetAvailableRideModes();
                SetupRideModeButtons();
            }
        }

        List<(string, ushort)> _availableRideModes = null;

        async void SetCustomShaping_Clicked(System.Object sender, System.EventArgs e)
        {
            await Application.Current.MainPage.DisplayAlert(null, "Under Construction", "OK");
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName.Equals(BoardTypeProperty.PropertyName))
            {
                if (BindingContext is OWBoard board)
                {
                    _availableRideModes = board.GetAvailableRideModes();
                    SetupRideModeButtons();
                }
            }
        }

        void SetupRideModeButtons()
        {
            _rideModesList.Clear();

            foreach (var rideMode in _availableRideModes)
            {
                _rideModesList.Add(new RideModeButton()
                {
                    Name = rideMode.Item1,
                    Value = rideMode.Item2,
                    IsSelected = false,
                });
            }

            //RideModesCollectionView.ItemsSource = null;
            //RideModesCollectionView.ItemsSource = _rideModesList;
            BindableLayout.SetItemsSource(RideModesStackLayout, null);
            BindableLayout.SetItemsSource(RideModesStackLayout, _rideModesList);
        }

        void RideModeButton_Clicked(System.Object sender, System.EventArgs e)
        {
            foreach (var button in _rideModesList)
            {
                button.IsSelected = false;
            }

            if (sender is Button senderButton)
            {
                var selectedRideMode = _rideModesList.FirstOrDefault(x => x.Name == senderButton.Text);
                if (selectedRideMode != null)
                {
                    selectedRideMode.IsSelected = true;

                    if (BindingContext is OWBoard board)
                    {
                        board.ChangeRideMode(selectedRideMode.Value);
                    }
                }


               //VisualStateManager.GoToState(button, selectedRideMode.IsSelected ? "SelectedState" : "NormalState");
            }
        }

        private async void OnSimpleStopToggled(object sender, ToggledEventArgs e)
        {
            if (sender is Switch simpleStopSwitch)
            {
                if (BindingContext is OWBoard board)
                {
                    await board.ToggleSimpleStop(simpleStopSwitch.IsToggled);
                }
            }
        }

        private async void OnTurnCompensationChanged(object sender, EventArgs e)
        {
            if (sender is RangeSlider slider)
            {
                TurnCompensationLabel.Text = "Turn Compensation: " + (int)Math.Round(slider.UpperValue);
                int turnCompensation = (int) turnCompensationConverter.ConvertBack((float)slider.UpperValue, null, null, null);

                if (BindingContext is OWBoard board)
                {
                    await board.ChangeTurnCompensation(turnCompensation);
                }
            }
        }

        private async void OnAggressivenessChanged(object sender, EventArgs e)
        {
            if (sender is RangeSlider slider)
            {
                AggressivenessLabel.Text = "Aggressiveness: " + (int)Math.Round(slider.UpperValue);
                int aggressivesness = (int)aggressivenessConverter.ConvertBack((float)slider.UpperValue, null, null, null);

                if (BindingContext is OWBoard board)
                {
                    await board.ChangeAggressiveness(aggressivesness);
                }
            }
        }

        private async void OnAngleOffsetChanged(object sender, EventArgs e)
        {
            if (sender is RangeSlider slider)
            {
                AngleOffsetLabel.Text = "Angle Offset: " + Math.Round(slider.UpperValue, 1);
                int angleOffset = (int)angleOffsetConverter.ConvertBack((float)slider.UpperValue, null, null, null);

                if (BindingContext is OWBoard board)
                {
                    await board.ChangeAngleOffset(angleOffset);
                }
            }
        }
    }
}
