using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class MainFlyoutMenuPage : ContentPage
    {

        public MainFlyoutMenuPage()
        {
            InitializeComponent();
        }

        private void Onewheels_Tapped(object sender, System.EventArgs e)
        {
            if (Parent is MainFlyoutPage mainFlyoutPage)
            {
                mainFlyoutPage.GoToBoardPage();
            }
        }

        private void MyRides_Tapped(object sender, System.EventArgs e)
        {
            if (Parent is MainFlyoutPage mainFlyoutPage)
            {
                mainFlyoutPage.GoToMyRidesPage();
            }
        }

        async void Leaderboards_Tapped(object sender, System.EventArgs e)
        {
            await DisplayAlert(String.Empty, "Not implemented yet.", "Cancel");
        }

        private void Settings_Tapped(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new SettingsPage()));
            /*
            if (Parent is MainFlyoutPage mainFlyoutPage)
            {
                mainFlyoutPage.GoToSettingsPage();
            }
            */
        }


        void StartRecording_Tapped(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Send<object>(this, "start_recording");
        }

        void StopRecording_Tapped(System.Object sender, System.EventArgs e)
        {
            MessagingCenter.Send<object>(this, "stop_recording");
        }


        public void Logs_Tapped(object sender, EventArgs e)
        {
            if (Parent is MainFlyoutPage mainFlyoutPage)
            {
                mainFlyoutPage.GoToLogsPage();
            }
        }
    }
}
