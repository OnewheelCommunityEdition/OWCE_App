using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using OWCE.Network;
using OWCE.Pages.Popup;
using OWCE.Protobuf;
using OWCE.Views;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class SubmitRidePage : BaseContentPage
    {
        Ride ride;
        SubmitRideModel model;

        public SubmitRidePage(Ride ride)
        {
            InitializeComponent();
            this.ride = ride;
            model = new SubmitRideModel(this)
            {
                RideName = ride.Name,
            };
            BindingContext = model;

            CustomToolbarItems.Add(new Views.CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Left,
                Text = "Cancel",
                Command = new AsyncRelayCommand(async () =>
                {
                    await Navigation.PopModalAsync();
                }),
            });


            CustomToolbarItems.Add(new Views.CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Right,
                Text = "Submit",
                Command = new AsyncRelayCommand(SubmitRide),
            });

        }

        async Task SubmitRide()
        {
            var filename = Path.Combine(App.Current.LogsDirectory, ride.DataFileName);
            if (File.Exists(filename) == false)
            {
                await DisplayAlert("Error", "Could not load saved ride.", "Okay");
                return;
            }
            var tempFilename = Path.Combine(Path.GetTempPath(), Path.GetFileName(ride.DataFileName));

            var cancellationTokenSource = new CancellationTokenSource();

            var uploadingAlert = new ProgressAlert(new AsyncCommand(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested == false)
                {
                    cancellationTokenSource.Cancel();
                }
            }));


            await PopupNavigation.Instance.PushAsync(uploadingAlert, true);

            var prepareResult = await Task.Run(() => PrepareData(filename, tempFilename, cancellationTokenSource));
            if (prepareResult == false)
            {
                await PopupNavigation.Instance.PopAsync();

                if (cancellationTokenSource.IsCancellationRequested == false)
                {
                    await DisplayAlert("Error", "Unable to prepare data for upload.", "Okay");
                }
                return;
            }


            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", App.Current.UserAgent);

                    var submitRideRequest = model.GetSubmitRideRequest();

                    var rawResponse = await httpClient.PostAsJsonAsync<SubmitRideRequest>($"https://{App.OWCEApiServer}/v1/ride/submit", submitRideRequest, cancellationTokenSource.Token);
                    if (rawResponse.IsSuccessStatusCode == false)
                    {
                        await PopupNavigation.Instance.PopAsync();
                        await DisplayAlert("Error", "Could not upload ride at this time. Please try again later. (ER001)", "Okay");
                        return;
                    }

                    var response = await rawResponse.Content.ReadFromJsonAsync<SubmitRideResponse>();
                    if (response == null)
                    {
                        await PopupNavigation.Instance.PopAsync();
                        await DisplayAlert("Error", "Could not upload ride at this time. Please try again later. (ER002)", "Okay");
                        return;
                    }

                    using (var fileStream = File.OpenRead(tempFilename))
                    {
                        /*
                        using (var customProgressableStreamContent = new CustomProgressableStreamContent(fileStream, progress))

                        var progress = new Progress<double>(percent =>
                        {
                            System.Diagnostics.Debug.WriteLine($"Upload progress: {percent}");
                        });
                        */

                        using (var streamContent = new StreamContent(fileStream))
                        {
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue("binary/octet-stream");
                            var request = new HttpRequestMessage(HttpMethod.Put, response.UploadUrl) { Content = streamContent };
                            var putResponse = await httpClient.SendAsync(request, new CancellationToken());

                            await PopupNavigation.Instance.PopAsync();

                            if (putResponse.IsSuccessStatusCode == false)
                            {
                                await DisplayAlert("Error", "Could not upload ride at this time. Please try again later. (ER004)", "Okay");
                                return;
                            }


                            await DisplayAlert("Success", "Thanks for submitting your ride.", "Okay");
                            await Navigation.PopModalAsync();
                        }
                    }
                }
                catch (Exception) when (cancellationTokenSource.IsCancellationRequested)
                {
                    await PopupNavigation.Instance.PopAsync();
                }
                catch (Exception err)
                {
                    await PopupNavigation.Instance.PopAsync();
                    await DisplayAlert("Error", "Could not upload ride at this time. Please try again later. (ER003)", "Okay");
                    System.Diagnostics.Debug.WriteLine($"SubmitRide Error: {err.Message}");
                }
            }
        }


        public async Task ViewDataSubmittedAsync()
        {
            var filename = Path.Combine(App.Current.LogsDirectory, ride.DataFileName);
            if (File.Exists(filename) == false)
            {
                await DisplayAlert("Error", "Could not load saved ride.", "Okay");
                return;
            }
            var tempFilename = Path.Combine(Path.GetTempPath(), Path.GetFileName(ride.DataFileName));

            var cancellationTokenSource = new CancellationTokenSource();
            
            var preparingAlert = new ProgressAlert(new Command(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested == false)
                {
                    cancellationTokenSource.Cancel();
                }
            }), "Preparing Data");


            await PopupNavigation.Instance.PushAsync(preparingAlert, true);

            var prepareResult = await Task.Run<bool>(() => { return PrepareData(filename, tempFilename, cancellationTokenSource); });
            if (prepareResult == false)
            {
                await PopupNavigation.Instance.PopAsync();

                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                await DisplayAlert("Error", "Unable to prepare data for preview.", "Okay");
                return;
            }

            if (cancellationTokenSource.IsCancellationRequested)
            {
                await PopupNavigation.Instance.PopAsync();
                return;
            }

            await Task.WhenAll(
                Navigation.PushModalAsync(new CustomNavigationPage(new ViewRawRideDataPage(model.GetSubmitRideRequest(), tempFilename))),
                PopupNavigation.Instance.PopAsync()
            );
        }


        public bool PrepareData(string originalData, string outputData, CancellationTokenSource cancellationTokenSource)
        {
            if (model.RemoveIdentifiers)
            {
                try
                {
                    var privateUUIDs = new List<string>()
                    {
                        OWBoard.SerialNumberUUID,
                        OWBoard.BatterySerialUUID,
                    };

                   
                    using (var inputFile = new FileStream(originalData, FileMode.Open, FileAccess.Read))
                    {
                        using (var outputFile = new FileStream(outputData, FileMode.Create))
                        {
                            var events = 0;
                            do
                            {
                                if (cancellationTokenSource.IsCancellationRequested)
                                {
                                    return false;
                                }

                                var currentEvent = OWBoardEvent.Parser.ParseDelimitedFrom(inputFile);
                                if (privateUUIDs.Contains(currentEvent.Uuid))
                                {
                                    // If private data, zero it out.
                                    currentEvent.Data = ByteString.CopyFrom(new byte[currentEvent.Data.Length]);
                                }
                                currentEvent.WriteDelimitedTo(outputFile);
                                ++events;
                            }
                            while (inputFile.Position < inputFile.Length);

                            return true;
                        }
                    }

                    
                }
                catch (Exception err)
                {
                    Debug.WriteLine($"PrepareData Error: {err.Message}");
                    return false;
                }
            }
            else
            {
                File.Copy(originalData, outputData, true);

                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return false;
                }

                return true;
            }
        }
    }
}

