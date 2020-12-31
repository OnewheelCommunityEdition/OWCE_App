using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace OWCE.Views
{
    public partial class PowerView : ContentView
    {
        public static readonly BindableProperty CurrentAmpsProperty = BindableProperty.Create(
            "CurrentAmps",
            typeof(double),
            typeof(PowerView));

        public double CurrentAmps
        {
            get { return (double)GetValue(CurrentAmpsProperty); }
            set
            {
                SetValue(CurrentAmpsProperty, value);
            }
        }


        public static readonly BindableProperty TripAmpHoursProperty = BindableProperty.Create(
            "TripAmpHours",
            typeof(double),
            typeof(PowerView));

        public double TripAmpHours
        {
            get { return (double)GetValue(TripAmpHoursProperty); }
            set
            {
                SetValue(TripAmpHoursProperty, value);
            }
        }


        public static readonly BindableProperty TripRegenAmpHoursProperty = BindableProperty.Create(
            "TripRegenAmpHours",
            typeof(double),
            typeof(PowerView));

        public double TripRegenAmpHours
        {
            get { return (double)GetValue(TripRegenAmpHoursProperty); }
            set
            {
                SetValue(TripRegenAmpHoursProperty, value);
            }
        }

        public PowerView()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (CurrentAmpsProperty.PropertyName.Equals(propertyName))
            {
                // TODO: Animate this.
                BackgroundBar.WidthRequest = (CurrentAmps / 20.0) * this.Width;
            }
        }
    }
}
