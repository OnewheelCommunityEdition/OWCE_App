﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        public SpeedRangeDistanceView()
        {
            InitializeComponent();
        }

        void MainArcView_SizeChanged(System.Object sender, System.EventArgs e)
        {
            if (MainArcView.Width > 0)
            {
                GridThing.RowDefinitions[0].Height = MainArcView.Width * 0.523465704f;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == BindingContextProperty.PropertyName)
            {
                var bindingContext = BindingContext;
            }
        }
    }
}