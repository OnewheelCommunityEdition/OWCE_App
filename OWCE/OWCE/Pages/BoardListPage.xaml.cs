using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
#if __ANDROID__
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Xamarin.Essentials;
#endif
using Xamarin.Forms;

namespace OWCE
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class BoardListPage : ContentPage
    {
        public ObservableCollection<OWBoard> Boards { get; internal set; } = new ObservableCollection<OWBoard>();

        // Used to dertermine if the board is still found.
        private List<OWBoard> _foundInLastScan = null;

        private bool _shouldKeepScanning = false;
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
                Name = "Onewheel unknown",
                BoardType = OWBoardType.Unknown,
            });
#endif
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            _selectedBoard = null;
            CrossBluetoothLE.Current.StateChanged += BLE_StateChanged;
            CrossBluetoothLE.Current.Adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
            CrossBluetoothLE.Current.Adapter.DeviceConnected += Adapter_DeviceConnected;
        }

        private CancellationTokenSource _scanCancellationToken;

        private async Task StartScanning()
        {
            if (_isScanning)
                return;

#if __ANDROID__
            if ((int)Android.OS.Build.VERSION.SdkInt >= 23)
            {
                var locationPermission = Plugin.Permissions.Abstractions.Permission.Location;
                var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(locationPermission);


                if (permissionStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    bool shouldRequest = await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(locationPermission);
                    if (shouldRequest)
                    {
                        await DisplayAlert("Oops", "In order to access board details in a bluetooth scan your phones location permission needs to be enabled.\n(Yeah, that is as confusing as it sounds)", "Ok");
                    }

                    var result = await CrossPermissions.Current.RequestPermissionsAsync(locationPermission);

                    permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(locationPermission);
                }

                if (permissionStatus == Plugin.Permissions.Abstractions.PermissionStatus.Denied)
                {
                    var shouldOpenSettings = await DisplayAlert("Error", "In order to access board details in a bluetooth scan your phones location permission needs to be enabled.\n(Yeah, that is as confusing as it sounds)", "Open Settings",  "Cancel");
                    if (shouldOpenSettings)
                    {
                        AppInfo.OpenSettings();
                    }
                    return;
                }

            }
#endif

            _isScanning = true;
            _shouldKeepScanning = true;

            ScanningHeader.IsVisible = true;
            NotScanningHeader.IsVisible = false;



            CrossBluetoothLE.Current.Adapter.ScanTimeout = 5 * 1000;
            try
            {
                _scanCancellationToken = new CancellationTokenSource();
                do
                {
                    _foundInLastScan = new List<OWBoard>();
                    System.Diagnostics.Debug.WriteLine("StartScan");
                    await CrossBluetoothLE.Current.Adapter.StartScanningForDevicesAsync(new Guid[] { new Guid(OWBoard.ServiceUUID) }, cancellationToken: _scanCancellationToken.Token);
                    System.Diagnostics.Debug.WriteLine("StopScan");
                    foreach (var board in Boards)
                    {
                        board.IsAvailable = _foundInLastScan.Contains(board);
                    }
                }
                while (_shouldKeepScanning && CrossBluetoothLE.Current.IsOn);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine("ScanError: " + err.Message);
            }
            finally
            {
                _isScanning = false;
            }
        }

        private void StopScanning()
        {
            ScanningHeader.IsVisible = false;
            NotScanningHeader.IsVisible = true;

            _shouldKeepScanning = false;
            _scanCancellationToken?.Cancel();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CrossBluetoothLE.Current.StateChanged -= BLE_StateChanged;
            CrossBluetoothLE.Current.Adapter.DeviceDiscovered -= Adapter_DeviceDiscovered;
            CrossBluetoothLE.Current.Adapter.DeviceConnected -= Adapter_DeviceConnected;
        }


        void Adapter_DeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Device connected {e.Device.Name} {e.Device.Id}");
            if (e.Device == _selectedBoard.Device && _selectedBoard != null)
            {
                Device.BeginInvokeOnMainThread(async () => {
                    await Navigation.PushAsync(new BoardPage(_selectedBoard));
                    Hud.Dismiss();
                });
            }
        }

        void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Device detected {e.Device.Name} {e.Device.Id}");

            var board = new OWBoard()
            {
                Name = e.Device.Name,
                ID = e.Device.Id.ToString(),
                IsAvailable = true,
                Device = e.Device,
            };

            _foundInLastScan.Add(board);

            var boardIndex = Boards.IndexOf(board);
            if (boardIndex == -1)
            {
                Boards.Add(board);
            }
            else
            {
                // Its odd that we set the name again, but when a board is just powered on its name is "Onewheel", not "ow123456"
                Boards[boardIndex].Name = e.Device.Name;
                Boards[boardIndex].IsAvailable = true;
                Boards[boardIndex].Device = e.Device;
            }
        }


        private void BLE_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"The bluetooth state changed to {e.NewState}");

            if (e.NewState == BluetoothState.On)
            {
                Device.BeginInvokeOnMainThread(async () => { await StartScanning(); });
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


                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        Hud.Show("Connecting", "Cancel", async delegate
                        {
                            foreach (var device in CrossBluetoothLE.Current.Adapter.ConnectedDevices)
                            {
                                await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(device);
                            }
                            _selectedBoard = null;
                            Hud.Dismiss();
                        });

                        await Task.Delay(250);

                        _shouldKeepScanning = false;
                        StopScanning();
                        if (CrossBluetoothLE.Current.Adapter.IsScanning)
                        {
                            await CrossBluetoothLE.Current.Adapter.StopScanningForDevicesAsync();
                        }



                        await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(board.Device);

                    });

                }
                else
                {
                    await DisplayAlert("Error", $"{board.Name} is not available.", "Cancel");
                }
            }
        }
    }
}
