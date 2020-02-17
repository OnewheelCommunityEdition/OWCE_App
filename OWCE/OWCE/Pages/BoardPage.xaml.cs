using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using RestSharp;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace OWCE.Pages
{
    public partial class BoardPage : ContentPage
    {
        public OWBoard Board { get; internal set; }

        public string SpeedHeader
        {
            get
            {
                var unit = App.Current.MetricDisplay ? "km/h" : "mph";
                return unit; // return $"Speed ({unit})";
            }
        }

        private bool _initialSubscirbe = false;
        public BoardPage(OWBoard board)
        {
            Board = board;
            board.Init();

            BindingContext = this;

            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

            ToolbarItems.Add(new ToolbarItem("Info", null, () =>
            {
                Navigation.PushModalAsync(new NavigationPage(new BoardDetailsPage(Board)));
            }));

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_initialSubscirbe == false)
            {
                _initialSubscirbe = true;
                _ = Board.SubscribeToBLE();
            }

            MessagingCenter.Subscribe<OWBoard>(this, "invalid_board_pint", async (board) =>
            {
                await DisplayAlert("Sorry", "Onewheel Pint is currently not supported.\n\nWell... it is, but the workaround to get it to work could get your IP blocked by Future Motion. Hopefully a workaround is available in the future.", "Ok");
                await DisconnectAndPop();
            });

            MessagingCenter.Subscribe<OWBoard>(this, "invalid_board_xr4141", async (board) =>
            {
                await DisplayAlert("Sorry", "Onewheel XR with firmware greater than 4141 is currently not supported. Well... it is, but the workaround to get it to work could get your IP blocked by Future Motion. Hopefully a workaround is available in the future.", "Ok");
                await DisconnectAndPop();
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<OWBoard>(this, "invalid_board_pint");
            MessagingCenter.Unsubscribe<OWBoard>(this, "invalid_board_xr4141");
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (GaugeAbsolueLayout.WidthRequest.AlmostEqualTo(width) == false)
            {
                GaugeAbsolueLayout.WidthRequest = width;
                GaugeAbsolueLayout.HeightRequest = width;
                GaugeAbsolueLayout.MinimumWidthRequest = width;
                GaugeAbsolueLayout.MinimumHeightRequest = width;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            DisconnectAndPop();
            return false;
        }

        async void Disconnect_Tapped(System.Object sender, System.EventArgs e)
        {
            var result = await DisplayActionSheet("Are you sure you want to disconnect?", "Cancel", "Disconnect");
            if (result == "Disconnect")
            {
                await DisconnectAndPop();
            }
        }

        private async Task DisconnectAndPop()
        {
            await Board.Disconnect();
            await Navigation.PopAsync();
        }

        private bool _isLogging = false;

        /*
        private async void LogData_Clicked(object sender, System.EventArgs e)
        {
            if (_isLogging)
            {
                LogDataButton.Text = "Start Logging Data";
                _isLogging = false;
                string zip = await Board.StopLogging();
                Hud.Dismiss();
                Hud.Show("Uploading");
                var client = new RestClient("https://owce.app");

                var request = new RestRequest("/upload_log.php", Method.POST);
                request.AddParameter("serial", Board.SerialNumber);
                request.AddParameter("ride_start", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                try
                {
                    var response = await client.ExecuteTaskAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        HttpWebRequest httpRequest = WebRequest.Create(response.Content) as HttpWebRequest;
                        httpRequest.Method = "PUT";
                        using (Stream dataStream = httpRequest.GetRequestStream())
                        {
                            var buffer = new byte[8000];
                            using (FileStream fileStream = new FileStream(zip, FileMode.Open, FileAccess.Read))
                            {
                                int bytesRead = 0;
                                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    dataStream.Write(buffer, 0, bytesRead);
                                }
                            }
                        }
                        HttpWebResponse uploadResponse = httpRequest.GetResponse() as HttpWebResponse;


                        Hud.Dismiss();
                        if (uploadResponse.StatusCode == HttpStatusCode.OK)
                        {
                            await DisplayAlert("Success", "Log file sucessfully uploaded.", "Ok");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Could not upload log at this time.", "Ok");
                        }
                    }
                    else
                    {
                        Hud.Dismiss();
                        await DisplayAlert("Error", "Could not upload log at this time.", "Ok");
                    }
                }
                catch (Exception err)
                {
                    Hud.Dismiss();
                    await DisplayAlert("Error", err.Message, "Ok");
                    // Log
                }
            }
            else
            {
                LogDataButton.Text = "Stop Logging Data";
                _isLogging = true;
                await Board.StartLogging();

            }

        }
        */

    }
}
