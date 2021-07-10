using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OWCE.Extensions;
using OWCE.Models;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace OWCE.Views
{
    public partial class BatteryView : ContentView
    {
        //	public delegate void BindingPropertyChangedDelegate<in TPropertyType> (BindableObject bindable, TPropertyType oldValue, TPropertyType newValue);


        public static readonly BindableProperty BatteryPercentProperty = BindableProperty.Create(
          "BatteryPercent",
          typeof(int),
          typeof(BatteryView),
          0,
          BindingMode.OneWay);







        /*
        private int _batteryPercent;
        public int BatteryPercent
        {

            get {
                return _batteryPercent;
            }
            set
            {
                _batteryPercent = value;
                OnPropertyChanged();
            }
        }
        */
        public int BatteryPercent
        {
            get
            {
                return (int)GetValue(BatteryPercentProperty);
            }
            set
            {
                SetValue(BatteryPercentProperty, value);
            }
        }



        public static readonly BindableProperty BatteryVoltageProperty = BindableProperty.Create(
          "BatteryVoltage",
          typeof(float),
          typeof(BatteryView),
          0f,
          BindingMode.OneWay);

        public float BatteryVoltage
        {
            get
            {
                return (float)GetValue(BatteryVoltageProperty);
            }
            set
            {
                SetValue(BatteryVoltageProperty, value);
            }
        }

        public static readonly BindableProperty BatteryCellsProperty = BindableProperty.Create(
          "BatteryCells",
          typeof(BatteryCells),
          typeof(BatteryView),
          null,
          BindingMode.OneWay);

        public BatteryCells BatteryCells
        {
            get
            {
                return (BatteryCells)GetValue(BatteryCellsProperty);
            }
            set
            {
                SetValue(BatteryCellsProperty, value);
            }
        }



        public BatteryView()
        {
            InitializeComponent();
            MainView.BindingContext = this;
        }

        /*
        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);

           

            Console.WriteLine($"OnPropertyChanging: {propertyName}");
        }
        */

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            //Console.WriteLine($"OnPropertyChanged: {propertyName}");
            if (BatteryPercentProperty.PropertyName.Equals(propertyName))
            {
                BatteryBar.AnimateWidthPercent((float)BatteryPercent * 0.01);
            }
            else if (BatteryCellsProperty.PropertyName.Equals(propertyName))
            {
                //BatteryCellsView.BindingContext = BatteryCells;
            }
        }

        /*
        private void BatteryCells_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine($"BatteryCells_PropertyChanged: {e.PropertyName}");
        }
        */

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
