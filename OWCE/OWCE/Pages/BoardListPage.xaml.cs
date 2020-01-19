using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE
{
    public partial class BoardListPage : ContentPage
    {
        public ObservableCollection<OWBoard> Boards { get; internal set; } = new ObservableCollection<OWBoard>();

        // Used to dertermine if the board is still found.
        private List<OWBoard> _foundInLastScan = null;

        private bool _isRefreshing = false;
        private bool _isScanning = false;


        private Command _refreshCommand;
        public Command RefreshCommand => _refreshCommand ?? (_refreshCommand = new Command(async () =>
        {
            DeviceListView.EndRefresh();
            await StartScanning();
        }));

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


        public BoardListPage()
        {
            InitializeComponent();

            BindingContext = this;

#if DEBUG
            Boards.Add(new OWBoard()
            {
                Name = "Onewheel v1",
                BoardType = OWBoardType.V1,
            });
            Boards.Add(new OWBoard()
            {
                Name = "Onewheel Plus",
                BoardType = OWBoardType.Plus,
            });
            Boards.Add(new OWBoard()
            {
                Name = "Onewheel XR",
                BoardType = OWBoardType.XR,
            });
            Boards.Add(new OWBoard()
            {
                Name = "Onewheel Pint",
                BoardType = OWBoardType.Pint,
            });
            Boards.Add(new OWBoard()
            {
                Name = "Onewheel unknown",
                BoardType = OWBoardType.Unknown,
            });
#endif
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            _selectedBoard = null;

            App.Current.OWBLE.BLEStateChanged += OWBLE_BLEStateChanged;
            App.Current.OWBLE.BoardDiscovered += OWBLE_BoardDiscovered;
            App.Current.OWBLE.BoardConnected += OWBLE_BoardConnected;
        }

        private CancellationTokenSource _scanCancellationToken;

        private async Task StartScanning()
        {
            if (_isScanning)
                return;

            if (App.Current.OWBLE.BluetoothEnabled() == false)
            {
                await DisplayAlert("Error", "Bluetooth is not enabled on your device. Please enable bluetooth and try scan for boards again.", "Ok");
                return;
            }

            if (await DependencyService.Get<DependencyInterfaces.IPermissionPrompt>().PromptBLEPermission() == false)
            {
                return;
            }

            _isScanning = true;
            ScanningHeader.IsVisible = true;
            NotScanningHeader.IsVisible = false;
            await App.Current.OWBLE.StartScanning();
            NotScanningHeader.IsVisible = true;
            ScanningHeader.IsVisible = false;
            _isScanning = false;
        }

        private void StopScanning()
        {
            App.Current.OWBLE.StopScanning();
            ScanningHeader.IsVisible = false;
            NotScanningHeader.IsVisible = true;
            _scanCancellationToken?.Cancel();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.Current.OWBLE.BLEStateChanged -= OWBLE_BLEStateChanged;
            App.Current.OWBLE.BoardDiscovered -= OWBLE_BoardDiscovered;
            App.Current.OWBLE.BoardConnected -= OWBLE_BoardConnected;
        }


        void OWBLE_BoardConnected(OWBoard board)
        {
            System.Diagnostics.Debug.WriteLine($"Device connected {board.Name} {board.ID}");
            if (board == _selectedBoard && _selectedBoard != null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Hud.Dismiss();
                    await Navigation.PushAsync(new BoardPage(_selectedBoard));
                });
            }
        }

        void OWBLE_BoardDiscovered(OWBoard board)
        {
            System.Diagnostics.Debug.WriteLine($"Device detected {board.Name} {board.ID}");
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


        private void OWBLE_BLEStateChanged(BluetoothState state)
        {
            if (state == BluetoothState.On)
            {
                _ = StartScanning();
            }
        }

        OWBoard _selectedBoard = null;
        public async void DeviceListView_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            DeviceListView.SelectedItem = null;

            if (e.SelectedItem is OWBoard board)
            {
                if (board.IsAvailable)
                {
                    _selectedBoard = board;

                    StopScanning();

                    Hud.Show("Connecting", "Cancel", delegate
                    {
                        App.Current.OWBLE.Disconnect();
                        _selectedBoard = null;
                        Hud.Dismiss();
                    });
                    
                    await Task.Delay(100);

                    try
                    {
                        var connectTask = App.Current.OWBLE.Connect(_selectedBoard);

                        var connected = await connectTask;
                    }
                    catch (TaskCanceledException )
                    {
                        _selectedBoard = null;
                        Hud.Dismiss();
                    }
                    catch (Exception )
                    {
                        await DisplayAlert("Error", $"Unable to connect to {board.Name}.", "Cancel");
                    }

                }
                else
                {
                    await DisplayAlert("Error", $"{board.Name} is not available.", "Cancel");
                }
            }
        }
    }
}
