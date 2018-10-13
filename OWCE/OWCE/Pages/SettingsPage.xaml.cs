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
            MetricDisplay.IsToggled = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);
            SpeedDemon.IsToggled = Preferences.Get("speed_demon", false);
        }

        void MetricDisplay_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            Preferences.Set("metric_display", e.Value);
        }

        void SpeedDemon_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            Preferences.Set("speed_demon", e.Value);
        }


    }
}
