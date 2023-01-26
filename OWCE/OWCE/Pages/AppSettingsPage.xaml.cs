using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.ObjectModel;
using OWCE.Views;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace OWCE.Pages
{
    public partial class AppSettingsPage : BaseContentPage
    {
        public bool MetricDisplay { get; set; }
        public bool AutoRideRecording { get; set; }

        public AppSettingsPage()
        {
            InitializeComponent();

            MetricDisplay = App.Current.MetricDisplay;
            AutoRideRecording = Preferences.Get("auto_ride_recording", false);

            CustomToolbarItems.Add(new CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Left,
                Text = "Cancel",
                Command = new AsyncCommand(async () =>
                {
                    await Navigation.PopModalAsync();
                }, allowsMultipleExecutions: false),
            });


            CustomToolbarItems.Add(new CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Right,
                Text = "Save",
                Command = new AsyncCommand(async () =>
                {
                    App.Current.MetricDisplay = MetricDisplay;

                    Preferences.Set("metric_display", MetricDisplay);
                    Preferences.Set("auto_ride_recording", AutoRideRecording);

                    MessagingCenter.Send<App>(App.Current, App.UnitDisplayUpdatedKey);

                    await Navigation.PopModalAsync();
                }, allowsMultipleExecutions: false),
            });

            BindingContext = this;

        }
    }
}

