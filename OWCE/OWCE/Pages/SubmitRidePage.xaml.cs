using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OWCE.Network;
using OWCE.Pages.Popup;
using OWCE.Views;
using Rg.Plugins.Popup.Services;
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
            model = new SubmitRideModel()
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


            var uploadingAlert = new UploadingAlert(new Command(() =>
            {
                // TODO Disconnect.
                //PopupNavigation.Instance.RemovePageAsync(_reconnectingAlert);
                //_reconnectingAlert = null;
            }));


            await PopupNavigation.Instance.PushAsync(uploadingAlert, true);
            
            if (model.RemoveIdentifiers)
            {
                // TODO: Remove things.
            }

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var submitRideRequest = new SubmitRideRequest()
                    {
                        RideName = model.RideName,
                        AftermarketBattery = model.IsAftermarketBattery,
                        BatteryType = model.BatteryType,
                        RemoveIdentifiers = model.RemoveIdentifiers,
                        AllowPublicly = model.AllowPublicly,
                        AdditionalNotes = model.AdditionalNotes,
                    };
                    var rawResponse = await httpClient.PostAsJsonAsync<SubmitRideRequest>($"https://{App.OWCEApiServer}/v1/ride/submit", submitRideRequest);
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

                    using (var fileStream = File.OpenRead(filename))
                    {
                        var progress = new Progress<double>(percent =>
                        {
                            System.Diagnostics.Debug.WriteLine($"Upload progress: {percent}");
                        });

                        using (var customProgressableStreamContent = new CustomProgressableStreamContent(fileStream, progress))
                        {
                            var putResponse = await httpClient.PutAsync(response.UploadUrl, customProgressableStreamContent, new CancellationToken());


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
                catch (Exception err)
                {
                    await PopupNavigation.Instance.PopAsync();
                    await DisplayAlert("Error", "Could not upload ride at this time. Please try again later. (ER003)", "Okay");
                    System.Diagnostics.Debug.WriteLine($"SubmitRide Error: {err.Message}");
                }
            }
        }
    }
}

