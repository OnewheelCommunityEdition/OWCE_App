using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public class MainMasterDetailPage : MasterDetailPage
    {
        private NavigationPage _mainBoardNavigationPage = null;

        public MainMasterDetailPage()
        {
            Master = new MainMasterPage();

            _mainBoardNavigationPage = new NavigationPage(new BoardListPage());

            Detail = _mainBoardNavigationPage;
        }

        public void GoToBoardPage()
        {
            Detail = _mainBoardNavigationPage;
            this.IsPresented = false;
        }

        public void GoToSettingsPage()
        {
            Detail = new NavigationPage(new SettingsPage());
            this.IsPresented = false;
        }

        internal void GoToMyRidesPage()
        {
            Detail = new NavigationPage(new MyRidesPage());
            this.IsPresented = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            bool showNote = false;
            bool neverRemindMe = Preferences.Get("third_party_remind_me_never", false);

            if (VersionTracking.IsFirstLaunchForCurrentBuild)
            {
                Preferences.Set("third_party_seen_message_on_this_version", false);
            }


            if (neverRemindMe == false && VersionTracking.IsFirstLaunchForCurrentBuild)
            {
                bool seenMessageOnThisVersion = Preferences.Get("third_party_seen_message_on_this_version", false);
                if (seenMessageOnThisVersion == false)
                {
                    showNote = true;
                }
            }

            
            if (showNote)
            {
                App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new ThirdPartyNotePage()));
            }
        }
    }
}

