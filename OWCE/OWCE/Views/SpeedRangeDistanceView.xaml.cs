using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace OWCE.Views
{
    public partial class SpeedRangeDistanceView : ContentView
    {

        public static readonly BindableProperty RPMProperty = BindableProperty.Create(
            "RPM",
            typeof(int),
            typeof(SpeedRangeDistanceView));

        public int RPM
        {
            get { return (int)GetValue(RPMProperty); }
            set
            {
                SetValue(RPMProperty, value);
            }
        }

        public static readonly BindableProperty WheelCircumferenceProperty = BindableProperty.Create(
            "WheelCircumference",
            typeof(float),
            typeof(SpeedRangeDistanceView));

        public float WheelCircumference
        {
            get { return (float)GetValue(WheelCircumferenceProperty); }
            set
            {
                SetValue(WheelCircumferenceProperty, value);
            }
        }


        public static readonly BindableProperty LifetimeOdometerProperty = BindableProperty.Create(
            "LifetimeOdometer",
            typeof(int),
            typeof(SpeedRangeDistanceView));

        public int LifetimeOdometer
        {
            get { return (int)GetValue(LifetimeOdometerProperty); }
            set { SetValue(LifetimeOdometerProperty, value); }
        }


        public static readonly BindableProperty TripOdometerProperty = BindableProperty.Create(
            "TripOdometer",
            typeof(int),
            typeof(SpeedRangeDistanceView));

        public int TripOdometer
        {
            get { return (int)GetValue(TripOdometerProperty); }
            set { SetValue(TripOdometerProperty, value); }
        }

        



        public SpeedRangeDistanceView()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == BindingContextProperty.PropertyName)
            {
                var bindingContext = BindingContext;
            }
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
    }
}
