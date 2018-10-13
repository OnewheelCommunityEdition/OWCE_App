using System;

using Xamarin.Forms;

namespace OWCE
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
    }
}

