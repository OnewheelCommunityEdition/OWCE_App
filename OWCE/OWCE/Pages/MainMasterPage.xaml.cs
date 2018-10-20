using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OWCE
{
    public partial class MainMasterPage : ContentPage
    {
        public MainMasterPage()
        {
            InitializeComponent();
        }

        private void Onewheels_Tapped(object sender, System.EventArgs e)
        {
            if (Parent is MainMasterDetailPage masterDetailPage)
            {
                masterDetailPage.GoToBoardPage();
            }
        }

        private void MyRides_Tapped(object sender, System.EventArgs e)
        {
            if (Parent is MainMasterDetailPage masterDetailPage)
            {
                masterDetailPage.GoToMyRidesPage();
            }
        }

        async void Leaderboards_Tapped(object sender, System.EventArgs e)
        {
            await DisplayAlert(String.Empty, "Not implemented yet.", "Cancel");
        }

        private void Settings_Tapped(object sender, System.EventArgs e)
        {
            if (Parent is MainMasterDetailPage masterDetailPage)
            {
                masterDetailPage.GoToSettingsPage();
            }
        }
    }
}
