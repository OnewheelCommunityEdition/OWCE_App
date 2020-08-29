using System;
using Rg.Plugins.Popup.Services;
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

            _mainBoardNavigationPage = new CustomNavigationPage(new BoardListPage());
            Detail = _mainBoardNavigationPage;

            //Detail = new NavigationPage(new ListLogsPage());

            this.MasterBehavior = MasterBehavior.Popover;
        }

        public void GoToBoardPage()
        {
            Detail = _mainBoardNavigationPage;
            this.IsPresented = false;
        }

        public void GoToLogsPage()
        {
            Detail = new NavigationPage(new ListLogsPage());
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

            if (VersionTracking.IsFirstLaunchForCurrentVersion)
            {
                var alert = new Popup.Alert("Onewheel Community Edition", "This is a third party app made by the community, for the community to give extra safety features & better data.\nThis is not the official app.It is not supported, endorsed or affiliated with Future Motion in any way.")
                {
                    ButtonText = "OK",
                };
                PopupNavigation.Instance.PushAsync(alert, true);
            }
        }
    }
}

