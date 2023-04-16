using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace OWCE.Pages.Popup
{
    public partial class SideMenuPopup : PopupPage
    {
        static SideMenuPopup _instance;
        public static SideMenuPopup Instance => _instance ??= new SideMenuPopup();

        IAsyncCommand _closeCommand;
        public IAsyncCommand CloseCommand => _closeCommand ??= new AsyncCommand(CloseCommand_Clicked, allowsMultipleExecutions: false);

        IAsyncCommand<Grid> _aboutCommand;
        public IAsyncCommand<Grid> AboutCommand => _aboutCommand ??= new AsyncCommand<Grid>(async (sender) => await AboutCommand_Clicked(sender), allowsMultipleExecutions: false);

        IAsyncCommand<Grid> _settingsCommand;
        public IAsyncCommand<Grid> SettingsCommand => _settingsCommand ??= new AsyncCommand<Grid>(async (sender) => await SettingsCommand_Clicked(sender), allowsMultipleExecutions: false);

        View _pageSpecificSideMenu = null;
        public View PageSpecificSideMenu {
            get
            {
                return _pageSpecificSideMenu;
            }
            set
            {
                if (_pageSpecificSideMenu != null)
                {
                    MainGrid.Children.Remove(_pageSpecificSideMenu);
                    _pageSpecificSideMenu = null;
                }

                if (value != null)
                {
                    _pageSpecificSideMenu = value;
                    Grid.SetRow(_pageSpecificSideMenu, 2);
                    MainGrid.Children.Add(_pageSpecificSideMenu);
                }
            }
        }

        private SideMenuPopup()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                HasSystemPadding = false;
            }

            BindingContext = this;
        }

        internal async Task CloseCommand_Clicked()
        {
            await PopupNavigation.Instance.PopAsync(true);
        }

        async Task AboutCommand_Clicked(Grid sender)
        {
            sender.Opacity = 0.6f;

            await Task.WhenAll(
                sender.FadeTo(1f),
                App.Current.MainPage.Navigation.PushModalAsync(new CustomNavigationPage(new AboutPage())),
                PopupNavigation.Instance.RemovePageAsync(this)
            );
        }

        async Task SettingsCommand_Clicked(Grid sender)
        {
            sender.Opacity = 0.6f;

            await Task.WhenAll(
                sender.FadeTo(1f),
                App.Current.MainPage.Navigation.PushModalAsync(new CustomNavigationPage(new AppSettingsPage())),
                PopupNavigation.Instance.RemovePageAsync(this)
            );
        }


        

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Content is Grid grid)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var safeInsets = On<iOS>().SafeAreaInsets();
                    grid.Padding = new Thickness(0, safeInsets.Top, 0, safeInsets.Bottom);
                }
            }
        }
    }
}
