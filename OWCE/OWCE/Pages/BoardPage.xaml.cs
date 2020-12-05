using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using RestSharp;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using OWCE.Pages.Popup;
using Rg.Plugins.Popup.Services;
using System.Linq;

namespace OWCE.Pages
{
    public partial class BoardPage : ContentPage
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

        public BoardPage(OWBoard board)
        {
            Board = board;
            //board.StartLogging();
            InitializeComponent();
            BindingContext = board;

            Board.Init();
            // I really don't like this.
            _ = Board.SubscribeToBLE();

            App.Current.OWBLE.BoardDisconnected += OWBLE_BoardDisconnected;
            App.Current.OWBLE.BoardReconnecting += OWBLE_BoardReconnecting;
            App.Current.OWBLE.BoardReconnected += OWBLE_BoardReconnected;


            NavigationPage.SetHasBackButton(this, false);
        }

        private void OWBLE_BoardDisconnected()
        {
            Console.WriteLine("OWBLE_BoardDisconnected");
        }

        private void OWBLE_BoardReconnecting()
        {
            Console.WriteLine("OWBLE_BoardReconnecting");
            
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
            Console.WriteLine("OWBLE_BoardReconnected");

            if (PopupNavigation.Instance.PopupStack.Contains(_reconnectingAlert))
            {
                PopupNavigation.Instance.RemovePageAsync(_reconnectingAlert);
                _reconnectingAlert = null;
            }
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();
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
            //DisconnectAndPop();
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

        public async Task DisconnectAndPop()
        {
            await App.Current.OWBLE.Disconnect();
            await Navigation.PopModalAsync();
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
