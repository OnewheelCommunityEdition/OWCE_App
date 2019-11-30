using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using RestSharp;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace OWCE
{
    public partial class BoardPage : ContentPage
    {
        public OWBoard Board { get; internal set; }

        public string SpeedHeader
        {
            get
            {
                var unit = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric) ? "kmph" : "mph";
                return $"Speed ({unit})";
            }
        }

        private bool _initialSubscirbe = false;
        public BoardPage(OWBoard board)
        {
            Board = board;

            BindingContext = this;

            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_initialSubscirbe == false)
            {
                _initialSubscirbe = true;
                _ = Board.SubscribeToBLE();
            }
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

        async void Disconnect_Clicked(object sender, System.EventArgs e)
        {
            await DisconnectAndPop();
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
