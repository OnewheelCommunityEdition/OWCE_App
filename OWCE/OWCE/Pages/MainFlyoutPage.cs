using System;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public class MainFlyoutPage : FlyoutPage
    {
        private NavigationPage _mainBoardNavigationPage = null;

        public MainFlyoutPage()
        {
            Flyout = new MainFlyoutMenuPage();

            _mainBoardNavigationPage = new CustomNavigationPage(new BoardListPage());
            Detail = _mainBoardNavigationPage;

            //Detail = new NavigationPage(new ListLogsPage());

            FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;
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
    }
}

