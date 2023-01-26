using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using OWCE.Pages.Popup;
using Rg.Plugins.Popup.Services;
using System.Linq;
using OWCE.Views;
using OWCE.DependencyInterfaces;
using Xamarin.CommunityToolkit.ObjectModel;

namespace OWCE.Pages
{
    public partial class BoardPage : BaseContentPage
    {
        ConnectingAlert _reconnectingAlert;


        public OWBoard Board { get; private set; }
        /*
        public string SpeedHeader
        {
            get
            {
                var unit = App.Current.MetricDisplay ? "km/h" : "mph";
                return unit; // return $"Speed ({unit})";
            }
        }*/

        private bool _initialSubscirbe = false;
        Grid _sideMenuItem = null;

        IAsyncCommand _startRecordRideCommand = null;
        public IAsyncCommand StartRecordRideCommand => _startRecordRideCommand ??= new AsyncCommand(StartRecordingAsync, allowsMultipleExecutions: false);

        IAsyncCommand _stopRecordRideCommand = null;
        public IAsyncCommand StopRecordRideCommand => _stopRecordRideCommand ??= new AsyncCommand(StopRecordingAsync, allowsMultipleExecutions: false);



        public BoardPage(OWBoard board) : base()
        {
            Board = board;

            InitializeComponent();
            BindingContext = board;

            AppVersionLabel.Text = $"{AppInfo.VersionString} (build {AppInfo.BuildString})";

            // TODO: Fix ImperialSwitch.IsToggled = !App.Current.MetricDisplay;


            Board.Init();
            // I really don't like this.
            _ = Board.SubscribeToBLE();

            App.Current.OWBLE.BoardDisconnected += OWBLE_BoardDisconnected;
            App.Current.OWBLE.BoardReconnecting += OWBLE_BoardReconnecting;
            App.Current.OWBLE.BoardReconnected += OWBLE_BoardReconnected;

            // Shift title to the right.
            var titleLabel = GetTitleLabel();
            titleLabel.HorizontalOptions = LayoutOptions.End;
            titleLabel.Padding = new Thickness(0, 0, 16, 0);


            var sideMenuItem = new CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Left,
                IconImageSource = "burger_menu",
                Command = new AsyncCommand(async () =>
                {
                    await PopupNavigation.Instance.PushAsync(Popup.SideMenuPopup.Instance);
                }, allowsMultipleExecutions: false),
            };
            CustomToolbarItems.Add(sideMenuItem);

        }

        private void OWBLE_BoardDisconnected()
        {
            System.Diagnostics.Debug.WriteLine("OWBLE_BoardDisconnected");
        }

        private void OWBLE_BoardReconnecting()
        {
            System.Diagnostics.Debug.WriteLine("OWBLE_BoardReconnecting");
            
            _reconnectingAlert = new ConnectingAlert(Board.Name, new Command(() =>
            {
                // TODO Disconnect.
                PopupNavigation.Instance.RemovePageAsync(_reconnectingAlert);
                _reconnectingAlert = null;
            }), "Reconnecting...");
            

            if (PopupNavigation.Instance.PopupStack.Contains(_reconnectingAlert) == false)
            {
                PopupNavigation.Instance.PushAsync(_reconnectingAlert, true);
            }

        }


        private void OWBLE_BoardReconnected()
        {
            System.Diagnostics.Debug.WriteLine("OWBLE_BoardReconnected");

            if (PopupNavigation.Instance.PopupStack.Contains(_reconnectingAlert))
            {
                PopupNavigation.Instance.RemovePageAsync(_reconnectingAlert);
                _reconnectingAlert = null;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Popup.SideMenuPopup.Instance.Title = "OWCE";

            if (_sideMenuItem == null)
            {
                var dataTemplate = (DataTemplate)Resources["SideMenu"];
                _sideMenuItem = dataTemplate.CreateContent() as Grid;
                _sideMenuItem.BindingContext = this;
            }
            Popup.SideMenuPopup.Instance.PageSpecificSideMenu = _sideMenuItem;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            /*
            if (GaugeAbsolueLayout.WidthRequest.AlmostEqualTo(width) == false)
            {
                GaugeAbsolueLayout.WidthRequest = width;
                GaugeAbsolueLayout.HeightRequest = width;
                GaugeAbsolueLayout.MinimumWidthRequest = width;
                GaugeAbsolueLayout.MinimumHeightRequest = width;
            }
            */
        }

        protected override bool OnBackButtonPressed()
        {
            Disconnect_Tapped(null, EventArgs.Empty);
            //DisconnectAndPop();
            return true;
        }

        async void Disconnect_Tapped(System.Object sender, System.EventArgs e)
        {
            var result = await DisplayActionSheet("Are you sure you want to disconnect?", "Cancel", "Disconnect");

            if (result == "Disconnect")
            {
                if (PopupNavigation.Instance.PopupStack.Any())
                {
                    await PopupNavigation.Instance.PopAllAsync();
                }
                await DisconnectAndPop();
            }
        }

        public async Task DisconnectAndPop()
        {
            await App.Current.OWBLE.Disconnect();

            Board.StopLogging();

            await Navigation.PopModalAsync();

            IWatch watchService = DependencyService.Get<IWatch>();

            watchService.StopListeningForWatchMessages();
        }


        void ImperialSwitch_IsToggledChanged(object sender, bool isToggled)
        {
            App.Current.MetricDisplay = !isToggled;
            Preferences.Set("metric_display", !isToggled);

            MessagingCenter.Send<App>(App.Current, App.UnitDisplayUpdatedKey);
        }

        async Task StartRecordingAsync()
        {
            await Popup.SideMenuPopup.Instance.CloseCommand_Clicked();
            Board.StartLogging();
        }

        async Task StopRecordingAsync()
        {
            await Popup.SideMenuPopup.Instance.CloseCommand_Clicked();
            Board.StopLogging();
        }
    }
}
