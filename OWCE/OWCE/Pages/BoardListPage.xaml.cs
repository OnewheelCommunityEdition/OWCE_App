using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
#if __ANDROID__
using Plugin.CurrentActivity;
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


        private Command _refreshCommand;
        public Command RefreshCommand => _refreshCommand ?? (_refreshCommand = new Command(async () =>
        {
            if (_isRefreshing == false)
            {
                _shouldKeepScanning = true;
                await ScanForBoards();
            }
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


#if __ANDROID__


            //  ScanForBoards();

            /*
            CrossCurrentActivity.Current.Activity.RequestPermissions(new string[] {
                Android.Manifest.Permission.Bluetooth,
                Android.Manifest.Permission.BluetoothAdmin,
                Android.Manifest.Permission.AccessFineLocation,
                Android.Manifest.Permission.AccessCoarseLocation,

        }, BLE_REQUEST_CODE);
        */
#endif

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CrossBluetoothLE.Current.StateChanged -= BLE_StateChanged;
            CrossBluetoothLE.Current.Adapter.DeviceDiscovered -= Adapter_DeviceDiscovered;
            CrossBluetoothLE.Current.Adapter.DeviceConnected -= Adapter_DeviceConnected;
        }

        private async Task ScanForBoards()
        {
            if (_isRefreshing)
                return;

            _isRefreshing = true;

            DeviceListView.BeginRefresh();



            CrossBluetoothLE.Current.Adapter.ScanTimeout = 2 * 1000;
            try
            {
                do
                {

                    _foundInLastScan = new List<OWBoard>();
                    System.Diagnostics.Debug.WriteLine("StartScan");
                    await CrossBluetoothLE.Current.Adapter.StartScanningForDevicesAsync(new Guid[] { new Guid(OWBoard.ServiceUUID) });
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
                int x = 0;
            }
            finally
            {
                _isRefreshing = false;
                DeviceListView.EndRefresh();
            }
        }

        async void Adapter_DeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Device connected {e.Device.Name} {e.Device.Id}");
            if (e.Device == _selectedBoard.Device && _selectedBoard != null)
            {
                Hud.Dismiss();
                await Navigation.PushAsync(new BoardPage(_selectedBoard));
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


        async void BLE_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"The bluetooth state changed to {e.NewState}");

            if (e.NewState == BluetoothState.On)
            {
                _shouldKeepScanning = true;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await ScanForBoards();
                });
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

                    _shouldKeepScanning = false;
                    if (CrossBluetoothLE.Current.Adapter.IsScanning)
                    {
                        await CrossBluetoothLE.Current.Adapter.StopScanningForDevicesAsync();
                    }


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
                    });



                    await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(board.Device);


                    /*
                    var service = await device.GetServiceAsync(new Guid(OWBoard.Service.ToLower()));
                    var hardwareVersionCharacteristic = await service.GetCharacteristicAsync(new Guid(OWBoard.HardwareVersion));
                    var data = await hardwareVersionCharacteristic.ReadAsync();
                    // If the OS we are on is a // IF the 
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(data);

                    var version = BitConverter.ToInt16(data, 0);
                    //int value = (data[0] << 8) | data[1];

                    */
                }
                else
                {
                    await DisplayAlert("Error", $"{board.Name} is not available.", "Cancel");
                }
            }
        }
    }
}
