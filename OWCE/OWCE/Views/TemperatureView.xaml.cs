using System;
using System.Collections.Generic;
using Xamarin.Forms;
using OWCE.Views;
using System.Runtime.CompilerServices;
using OWCE.Extensions;

namespace OWCE.Views
{
    public partial class TemperatureView : ContentView
    {
        public const float MinTemp = 15;
        public const float MaxTemp = 72;
        public const float TempRange = MaxTemp - MinTemp;


        public static readonly BindableProperty BatteryTempProperty = BindableProperty.Create(nameof(BatteryTemp), typeof(float), typeof(TemperatureView), default);
        public float BatteryTemp
        {
            get => (float)GetValue(BatteryTempProperty);
            set => SetValue(BatteryTempProperty, value);
        }

        public static readonly BindableProperty MotorTempProperty = BindableProperty.Create(nameof(MotorTemp), typeof(float), typeof(TemperatureView), default);
        public float MotorTemp
        {
            get => (float)GetValue(MotorTempProperty);
            set => SetValue(MotorTempProperty, value);
        }

        public static readonly BindableProperty ControllerTempProperty = BindableProperty.Create(nameof(ControllerTemp), typeof(float), typeof(TemperatureView), default);
        public float ControllerTemp
        {
            get => (float)GetValue(ControllerTempProperty);
            set => SetValue(ControllerTempProperty, value);
        }

        public TemperatureView()
        {
            InitializeComponent();
            MainView.BindingContext = this;

            MessagingCenter.Subscribe<App>(this, App.UnitDisplayUpdatedKey, (app) =>
            {
                OnPropertyChanged(nameof(BatteryTemp));
                OnPropertyChanged(nameof(MotorTemp));
                OnPropertyChanged(nameof(ControllerTemp));
            });
        }

        

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (ControllerTempProperty.PropertyName.Equals(propertyName))
            {
                var percent = (ControllerTemp - MinTemp) / TempRange;
                ControllerBarView.AnimateWidthPercent(percent);
            }
            else if (MotorTempProperty.PropertyName.Equals(propertyName))
            {
                var percent = (MotorTemp - MinTemp) / TempRange;
                MotorBarView.AnimateWidthPercent(percent);
            }
            else if (BatteryTempProperty.PropertyName.Equals(propertyName))
            {
                var percent = (BatteryTemp - MinTemp) / TempRange;
                BatteryBarView.AnimateWidthPercent(percent);
            }
        }
    }
}
