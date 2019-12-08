using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            MetricDisplay.IsToggled = App.Current.MetricDisplay;
            SpeedDemon.IsToggled = App.Current.SpeedDemon;
        }

        void MetricDisplay_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            App.Current.MetricDisplay = e.Value;
            Preferences.Set("metric_display", e.Value);
        }

        void SpeedDemon_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            App.Current.SpeedDemon = e.Value;
            Preferences.Set("speed_demon", e.Value);
        }
    }
}
