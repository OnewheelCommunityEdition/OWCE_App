using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using OWCE.DependencyInterfaces;
using OWCE.Pages.Popup;
using OWCE.Views;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace OWCE.Pages
{
    public partial class BoardListPage : BaseContentPage
    {
        public ObservableCollection<OWBaseBoard> Boards { get; private set; } = new ObservableCollection<OWBaseBoard>();

        // Used to dertermine if the board is still found.
        private List<OWBoard> _foundInLastScan = null;

        private bool _isRefreshing = false;


        /*
        private bool _isScanning = false;
        public bool IsScanning
        {
            get { return _isScanning; }
            private set
            {
                if (value != _isScanning )
                {
                    _isScanning = value;
                    OnPropertyChanged();
                }
            }
        }
        */


        //private OWBaseBoard _selectedBoard = null;


        private Command _refreshCommand;
        public Command RefreshCommand => _refreshCommand ?? (_refreshCommand = new Command(async () =>
        {
            MainRefreshView.IsRefreshing = false;

            if (App.Current.OWBLE.IsScanning == false)
            {
                await StartScanning();
            }
        }));

        AsyncCommand<OWBaseBoard> _boardSelectedCommand;
        public AsyncCommand<OWBaseBoard> BoardSelectedCommand => _boardSelectedCommand ??= new AsyncCommand<OWBaseBoard>(BoardSelectedAsync, allowsMultipleExecutions: false);

       
        

        /*

        private Command _startScanningTapCommand;
        public Command StartScanningTapCommand => _startScanningTapCommand ?? (_startScanningTapCommand = new Command(async () =>
        {
            await StartScanning();
        }));

        private Command _stopScanningTapCommand;
        public Command StopScanningTapCommand => _stopScanningTapCommand ?? (_stopScanningTapCommand = new Command(() =>
        {
            StopScanning();
        }));
        */

        //IOWScanner _owScanner;
        //public IOWScanner OWScanner => _owScanner;

        //PastRidesCommand
        AsyncCommand _pastRidesCommand;
        public AsyncCommand PastRidesCommand => _pastRidesCommand ??= new AsyncCommand(PastRidesCommand_Clicked, allowsMultipleExecutions: false);

        Grid _scanningView;

        public BoardListPage() : base()
        {

            InitializeComponent();
            BindingContext = this;

            _scanningView = new Grid()
            {
                HorizontalOptions = LayoutOptions.End,
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition() { Width = new GridLength(26, GridUnitType.Absolute) },
                },
                ColumnSpacing = 18,
            };
            _scanningView.BindingContext = App.Current.OWBLE;
            _scanningView.SetBinding(Grid.IsVisibleProperty, "IsScanning");

            var scanningLabel = new Label()
            {
                Text = "Scanning...",
                TextColor = Color.Black,
                FontFamily = "SairaExtraCondensed-SemiBold",
                FontSize = 24,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };

            var scanningActivityIndicator = new ActivityIndicator()
            {
                WidthRequest = 26,
                HeightRequest = 26,
                Color = Color.Black,
                IsRunning = true,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,

            };

            Grid.SetColumn(scanningLabel, 0);
            Grid.SetColumn(scanningActivityIndicator, 1);

            _scanningView.Children.Add(scanningLabel);
            _scanningView.Children.Add(scanningActivityIndicator);

            var scanningToolbarItem = new CustomToolbarItem();
            scanningToolbarItem.Content = _scanningView;
            CustomToolbarItems.Add(scanningToolbarItem);

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

            /*
#if DEBUG
            var rand = new Random();
            Boards.Add(new MockOWBoard($"ow{rand.Next(111111, 999999)}", OWBoardType.V1));
            Boards.Add(new MockOWBoard($"ow{rand.Next(111111, 999999)}", OWBoardType.Plus));
            Boards.Add(new MockOWBoard($"ow{rand.Next(111111, 999999)}", OWBoardType.XR));
            Boards.Add(new MockOWBoard($"ow{rand.Next(111111, 999999)}", OWBoardType.Pint));
            Boards.Add(new MockOWBoard($"ow{rand.Next(111111, 999999)}", OWBoardType.Unknown));
#endif
            */
        }

        Thickness _safeInsets;

        bool _hasAppeared = false;
        Grid _sideMenuItem;

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Popup.SideMenuPopup.Instance.Title = "OWCE";


            if (_sideMenuItem == null)
            {
                var dataTemplate = (DataTemplate)Resources["SideMenu"];
                _sideMenuItem = dataTemplate.CreateContent() as Grid;
                _sideMenuItem.BindingContext = BindingContext;
            }
            Popup.SideMenuPopup.Instance.PageSpecificSideMenu = _sideMenuItem;


            //App.Current.OWBLE.BLEStateChanged += OWBLE_BLEStateChanged;
            //App.Current.OWBLE.BoardDiscovered += OWBLE_BoardDiscovered;
            //App.Current.OWBLE.BoardConnected += OWBLE_BoardConnected;


            BackgroundLogoImage.Margin = new Thickness(0, (_safeInsets.Top + _safeInsets.Bottom), 0, 0);
            //BackgroundImage


            //var board = new OWBoard(new OWBaseBoard("000000", "ow000000"));
            //(Path.Combine(App.Current.LogsDirectory, "25 July 2020 03/53/40 PM.bin"));

            // Navigation.PushAsync(new BoardPage(board));

            if (_hasAppeared == false)
            {
                _hasAppeared = true;


                App.Current.OWBLE.ErrorOccurred += OWBLE_ErrorOccurred;
                App.Current.OWBLE.BoardDiscovered += OWBLE_BoardDiscovered;

                // If this is the first launch of the current app we want to re-alert the user that this is a community driven app.
                if (VersionTracking.IsFirstLaunchForCurrentVersion)
                {
                    var alert = new Popup.Alert("Onewheel Community Edition", "This is a third party app made by the community, for the community to give extra safety features & better data.\nThis is not the official app. It is not supported, endorsed or affiliated with Future Motion in any way.")
                    {
                        ButtonText = "OK",
                    };
                    await PopupNavigation.Instance.PushAsync(alert, true);

                    // Additionally if this is also the first launch ever, lets prompt them for bluetooth after they have dismissed the initial alert.
                    if (VersionTracking.IsFirstLaunchEver)
                    {
                        alert.Disappearing += (sender, e) =>
                        {
                            var bluetoothPleaseAlert = new Popup.Alert("Bluetooth, please", "OWCE and your Onewheel use Bluetooth to communicate. We need your permission to connect.", new Command(async (object parameter) =>
                            {
                                if (parameter is Popup.Alert alertPage)
                                {
                                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.RemovePageAsync(alertPage);
                                    await StartScanning();
                                }
                            }))
                            {
                                SuperTitleText = "Welcome",
                                ButtonText = "OK",
                            };
                            PopupNavigation.Instance.PushAsync(bluetoothPleaseAlert, true);
                        };
                    }
                    else
                    {
                        await StartScanning();
                    }
                }
            }

            if (await App.Current.OWBLE.ReadyToScan())
            {
                await StartScanning();
            }
        }

        private async Task StartScanning()
        {
            if (App.Current.OWBLE.IsScanning)
                return;

            /*
            if (App.Current.OWBLE.BluetoothEnabled() == false)
            {
                await DisplayAlert("Error", "Bluetooth is not enabled on your device. Please enable bluetooth and try scan for boards again.", "Ok");
                return;
            }
            */

            if (await DependencyService.Get<DependencyInterfaces.IPermissionPrompt>().PromptBLEPermission() == false)
            {
                return;
            }

            try
            {
                App.Current.OWBLE.StartScanning();
            }
            catch (Exception)
            {
                var alert = new Pages.Popup.Alert("Error", "Could not scan for boards. Please ensure bluetooth is enabled and has correct permission to scan.");
                await PopupNavigation.Instance.PushAsync(alert, true);
            }
        }

        private void StopScanning()
        {
            App.Current.OWBLE.StopScanning();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //App.Current.OWBLE.ErrorOccurred -= OWBLE_ErrorOccurred;
            //App.Current.OWBLE.BoardDiscovered -= OWBLE_BoardDiscovered;

            //App.Current.OWBLE.BLEStateChanged -= OWBLE_BLEStateChanged;
            //App.Current.OWBLE.BoardDiscovered -= OWBLE_BoardDiscovered;
            //App.Current.OWBLE.BoardConnected -= OWBLE_BoardConnected;
        }

        void OWBLE_ErrorOccurred(string message)
        {
            Device.InvokeOnMainThreadAsync(async () =>
            {
                var alert = new Pages.Popup.Alert("Error", message);
                await PopupNavigation.Instance.PushAsync(alert, true);
            });
        }

        void OWBLE_BoardDiscovered(OWBaseBoard board)
        {
            Debug.WriteLine($"OWBLE_BoardDiscovered: {board.Name} {board.ID}");
            var boardIndex = Boards.IndexOf(board);
            if (boardIndex == -1)
            {
                Boards.Add(board);
            }
            else
            {
                // Its odd that we set the name again, but when a board is just powered on its name is "Onewheel", not "ow123456"
                Boards[boardIndex].Name = board.Name;
                Boards[boardIndex].IsAvailable = true;
                Boards[boardIndex].NativePeripheral = board.NativePeripheral;
            }
        }

        /*
        void OWBLE_BoardConnected(OWBoard board)
        {
            System.Diagnostics.Debug.WriteLine($"Device connected {board.Name} {board.ID}");
            if (_selectedBoard != null && _selectedBoard.Equals(board))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Hud.Dismiss();
                    await Navigation.PushAsync(new BoardPage(board));
                });
            }
        }
        */





        async Task BoardSelectedAsync(OWBaseBoard baseBoard)
        {
            if (baseBoard == null)
            {
                return;
            }

            if (baseBoard.IsAvailable)
            {
                StopScanning();

                //_selectedBoard = board;

                var cancellationTokenSource = new CancellationTokenSource();

                var connectingAlert = new Popup.ConnectingAlert(baseBoard.Name, new Command(() =>
                {
                    Debug.WriteLine("Connecting alert: cancel clicked");
                    if (cancellationTokenSource.IsCancellationRequested == false)
                    {
                        cancellationTokenSource.Cancel();
                        //_selectedBoard = null;
                        //App.Current.OWBLE.Disconnect();
                    }
                }));
                await PopupNavigation.Instance.PushAsync(connectingAlert, true);

                var board = await App.Current.ConnectToBoard(baseBoard, cancellationTokenSource.Token);
                await PopupNavigation.Instance.PopAllAsync();
                if (board != null)
                {
                    await Navigation.PushModalAsync(new CustomNavigationPage(new BoardPage(board)));
                    // Publish notification that board was connected
                    IWatch watchService = DependencyService.Get<IWatch>();
                    watchService.ListenForWatchMessages(board);
                }
                /*
                try
                {
                    var connectTask = App.Current.OWBLE.Connect(_selectedBoard);

                    var connected = await connectTask;
                }
                catch (TaskCanceledException)
                {
                    _selectedBoard = null;
                    Hud.Dismiss();
                }
                catch (Exception)
                {
                    await DisplayAlert("Error", $"Unable to connect to {board.Name}.", "Cancel");
                }
                */
            }
            else
            {
                var alert = new Pages.Popup.Alert("Error", $"{baseBoard.Name} is not available.");
                await PopupNavigation.Instance.PushAsync(alert, true);
            }
        }

        async Task PastRidesCommand_Clicked()
        {
            await Task.WhenAll(
               Navigation.PushAsync(new PastRidesPage()),
               PopupNavigation.Instance.RemovePageAsync(SideMenuPopup.Instance)
           );
        }

    }
}