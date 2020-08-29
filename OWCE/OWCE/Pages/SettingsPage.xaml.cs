using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private bool _ignoreAlerts = false;

        public SettingsPage()
        {
            InitializeComponent();

            _ignoreAlerts = true;
            MetricDisplay.IsToggled = App.Current.MetricDisplay;
            _ignoreAlerts = false;
            ToolbarItems.Add(new ToolbarItem("Cancel", null, () =>
            {
                Navigation.PopModalAsync();
            }));
        }

        void MetricDisplay_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            App.Current.MetricDisplay = e.Value;
            Preferences.Set("metric_display", e.Value);

            if (_ignoreAlerts)
                return;

            DisplayAlert("Oops", "Please disconnect and reconnect from your board for this change to apply.\n\nThis will be fixed in the future.", "Ok");
        }

       
    }
}
