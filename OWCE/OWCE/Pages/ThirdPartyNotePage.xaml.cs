using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class ThirdPartyNotePage : ContentPage
    {
        public ThirdPartyNotePage()
        {
            InitializeComponent();
        }

        async void ShowOfficial_Tapped(System.Object sender, System.EventArgs e)
        {
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await Browser.OpenAsync("https://apps.apple.com/au/app/onewheel/id946642160", BrowserLaunchMode.External);
            }
            else if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                await Browser.OpenAsync("https://play.google.com/store/apps/details?id=com.rideonewheel.onewheel", BrowserLaunchMode.External);
            }
            else
            {
                await DisplayAlert("Oops", "Unable to take you to the offiical store for your platform. Sending you to the Onewheel homepage instead.", "Ok");
                await Browser.OpenAsync("https://onewheel.com", BrowserLaunchMode.External);
            }
        }

        void LetMeIn_Tapped(System.Object sender, System.EventArgs e)
        {
            Preferences.Set("third_party_seen_message_on_this_version", true);
            ProceedToApp();
        }
        
        void LetMeInForever_Tapped(System.Object sender, System.EventArgs e)
        {
            Preferences.Set("third_party_remind_me_never", true);
            ProceedToApp();
        }

        async void ProceedToApp()
        {
            // This method works great for iOS, but on Android it flashes the screen which is annoying.

            var newPage = new NavigationPage(new BoardListPage());
            await Navigation.PushModalAsync(newPage);
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Android will crash without first popping modal.
                await Navigation.PopModalAsync(false);
            }
            ((App)Application.Current).MainPage = newPage;
            

            //await Navigation.PushAsync(new BoardListPage());
            //Navigation.RemovePage(this);
        }
    }
}
