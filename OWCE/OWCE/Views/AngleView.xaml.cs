using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace OWCE.Views
{
    public partial class AngleView : ContentView
    {
        public static readonly BindableProperty PitchProperty = BindableProperty.Create(
            "Pitch",
            typeof(double),
            typeof(AngleView));

        public double Pitch
        {
            get { return (double)GetValue(PitchProperty); }
            set {
                SetValue(PitchProperty, value);
            }
        }

        public static readonly BindableProperty YawProperty = BindableProperty.Create(
            "Yaw",
            typeof(double),
            typeof(AngleView));

        public double Yaw
        {
            get { return (double)GetValue(YawProperty); }
            set {
                SetValue(YawProperty, value);
            }
        }

        public static readonly BindableProperty RollProperty = BindableProperty.Create(
            "Roll",
            typeof(double),
            typeof(AngleView));

        public double Roll
        {
            get { return (double)GetValue(RollProperty); }
            set {
                SetValue(RollProperty, value);
            }
        }

        public AngleView()
        {
            InitializeComponent();
            MainView.BindingContext = this;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (PitchProperty.PropertyName.Equals(propertyName))
            {
                PitchView.RotateTo(Pitch, 100, Easing.CubicInOut);
            }
            else if (YawProperty.PropertyName.Equals(propertyName))
            {
                YawView.RotateTo(Yaw, 100, Easing.CubicInOut);
            }
            else if (RollProperty.PropertyName.Equals(propertyName))
            {
                RollView.RotateTo(Roll, 100, Easing.CubicInOut);
            }
        }

    }
}
