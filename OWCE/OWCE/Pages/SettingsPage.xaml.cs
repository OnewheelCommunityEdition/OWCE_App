using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private bool _ignoreAlerts = false;

        private int lastValidSpeedReportingMinimum;
        private int lastValidSpeedReportingBaselineTimeout;

        public SettingsPage()
        {
            InitializeComponent();

            _ignoreAlerts = true;
            MetricDisplay.IsToggled = App.Current.MetricDisplay;
            SpeedReporting.IsToggled = App.Current.SpeedReporting;
            BatteryPercentReporting.IsToggled = App.Current.BatteryPercentReporting;
            BatteryPercentInferredBasedOnVoltage.IsToggled = App.Current.BatteryPercentInferredBasedOnVoltage;

            lastValidSpeedReportingMinimum = App.Current.SpeedReportingMinimum; // TODO: Hook up settings /w new UI
            SpeedReportingMinimum.Text = App.Current.SpeedReportingMinimum.ToString();

            lastValidSpeedReportingBaselineTimeout = App.Current.SpeedReportingBaselineTimeout;
            SpeedReportingBaselineTimeout.Text = App.Current.SpeedReportingBaselineTimeout.ToString();

            _ignoreAlerts = false;
            ToolbarItems.Add(new ToolbarItem("Cancel", null, () =>
            {
                Navigation.PopModalAsync();
            }));
        }

        private void MetricDisplay_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            App.Current.MetricDisplay = e.Value;
            Preferences.Set("metric_display", e.Value);

            if (_ignoreAlerts)
                return;

            DisplayAlert("Oops", "Please disconnect and reconnect from your board for this change to apply.\n\nThis will be fixed in the future.", "Ok");
        }

        private void SpeedReporting_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            App.Current.SpeedReporting = e.Value;
            Preferences.Set("speed_reporting", e.Value);

            if (_ignoreAlerts)
                return;

            DisplayAlert("Oops", "Please disconnect and reconnect from your board for this change to apply.\n\nThis will be fixed in the future.", "Ok");
        }

        private void SpeedReportingMinimum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Int32.TryParse(e.NewTextValue, out int newValue) && newValue > 0)
            {
                lastValidSpeedReportingMinimum = newValue;
            }
        }

        private void SpeedReportingMinimum_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = e.VisualElement as Entry;
            if (entry == null)
            {
                throw new InvalidProgramException("Is the callback attached to an element that is not an Entry?");
            }

            entry.Text = lastValidSpeedReportingMinimum.ToString();
            App.Current.SpeedReportingMinimum = lastValidSpeedReportingMinimum;
            Preferences.Set("speedreporting_minimum", lastValidSpeedReportingMinimum);
        }

        private void SpeedReportingBaselineTimeout_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Int32.TryParse(e.NewTextValue, out int newValue) && newValue > 0)
            {
                lastValidSpeedReportingBaselineTimeout = newValue;
            }
        }

        private void SpeedReportingBaselineTimeout_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = e.VisualElement as Entry;
            if (entry == null)
            {
                throw new InvalidProgramException("Is the callback attached to an element that is not an Entry?");
            }

            entry.Text = lastValidSpeedReportingBaselineTimeout.ToString();
            App.Current.SpeedReportingBaselineTimeout = lastValidSpeedReportingBaselineTimeout;
            Preferences.Set("speedreporting_baseline_timeout", lastValidSpeedReportingBaselineTimeout);
        }

        private void BatteryPercentReporting_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            App.Current.BatteryPercentReporting = e.Value;
            Preferences.Set("batterypercent_reporting", e.Value);

            if (_ignoreAlerts)
                return;

            DisplayAlert("Oops", "Please disconnect and reconnect from your board for this change to apply.\n\nThis will be fixed in the future.", "Ok");
        }

        private void BatteryPercentInferredBasedOnVoltage_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            App.Current.BatteryPercentInferredBasedOnVoltage = e.Value;
            Preferences.Set("batterypercent_inferred_voltage", e.Value);

            if (_ignoreAlerts)
                return;

            DisplayAlert("Oops", "Please disconnect and reconnect from your board for this change to apply.\n\nThis will be fixed in the future.", "Ok");
        }
    }
}
