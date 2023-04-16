using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OWCE.Converters;
using OWCE.Network;
using OWCE.Pages.Popup;
using OWCE.Protobuf;
using OWCE.Views;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.AppleSignInAuthenticator;

namespace OWCE.Pages
{	
	public partial class ViewRawRideDataPage : BaseContentPage
    {
     
        public ViewRawRideDataPage(SubmitRideRequest submitRideRequest, string dataFile)
		{
			InitializeComponent();

            var model = new ViewRawRideDataModel(this)
            {
                SubmitRideRequest = submitRideRequest,
                DataFile = dataFile,
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
                //Text = "Share",
                IconImageSource = "icon_share",
                Command = new AsyncRelayCommand(ExportAsync),
            });

        }

        bool _hasFirstLoad = false;
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_hasFirstLoad == false)
            {
                _hasFirstLoad = true;
                if (BindingContext is ViewRawRideDataModel model)
                {
                    model.LoadData();
                }
            }
        }



        async Task<bool> ExportAsync()
        {
            var cancelButton = "Cancel";
            var jsonButton = "json";
            var csvButton = "csv";
            var originalButton = "original";
            var exportFormats = new string[]
                    {
                        jsonButton,
                        csvButton,
                        originalButton
                    };

            var exportFormat = await DisplayActionSheet("Export format", cancelButton, null, exportFormats);
            if (String.IsNullOrEmpty(exportFormat) || exportFormat == cancelButton)
            {
                return true;
            }
            else if (exportFormat == originalButton)
            {
                if (BindingContext is ViewRawRideDataModel model)
                {
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = Path.GetFileName(model.DataFile),
                        File = new ShareFile(model.DataFile)
                    });
                }
                return true;
            }

            var cancellationTokenSource = new CancellationTokenSource();

            var exportingPopup = new ProgressAlert(new AsyncCommand(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested == false)
                {
                    cancellationTokenSource.Cancel();
                }
            }), "Exporting");
            await PopupNavigation.Instance.PushAsync(exportingPopup, true);

            var taskCompletionSource = new TaskCompletionSource<bool>();

            ThreadPool.QueueUserWorkItem(delegate
            {
                if (BindingContext is ViewRawRideDataModel model)
                {
                    var uuidToNameConverter = new UUIDToNameConverter();

                    var baseFilename = Path.GetFileNameWithoutExtension(model.DataFile);
                    if (String.IsNullOrEmpty(model.SubmitRideRequest.RideName) == false)
                    {
                        var newBaseFilename = model.SubmitRideRequest.RideName;
                        newBaseFilename = newBaseFilename.Replace("/", "-");
                        newBaseFilename = newBaseFilename.Replace("\\", "-");
                        newBaseFilename = newBaseFilename.Replace(":", "-");


                        var listOfCharactersToRemove = new string[] {
                            "|",
                            "?",
                            "*",
                            "<",
                            ">",
                            "+",
                            "[",
                            "]",
                            "'",
                            "\"",
                        };
                        foreach (var characterToRemove in listOfCharactersToRemove)
                        {
                            newBaseFilename = newBaseFilename.Replace(characterToRemove, String.Empty);
                        }

                        if (String.IsNullOrEmpty(newBaseFilename) == false)
                        {
                            baseFilename = newBaseFilename;
                        }
                    }

                    if (exportFormat == jsonButton)
                    {
                        var outputJSON = Path.Combine(Path.GetTempPath(), $"{baseFilename}.json");

                        using (var streamWriter = new StreamWriter(outputJSON, false))
                        {
                            using (var jsonWriter = new Utf8JsonWriter(streamWriter.BaseStream, new JsonWriterOptions() { Indented = true }))
                            {
                                jsonWriter.WriteStartArray();

                                foreach (var boardEvent in model.BoardEvents)
                                {
                                    if (cancellationTokenSource.IsCancellationRequested)
                                    {
                                        break;
                                    }

                                    jsonWriter.WriteStartObject();
                                    jsonWriter.WriteNumber("timestamp", boardEvent.Timestamp);
                                    jsonWriter.WriteString("property_uuid", boardEvent.Uuid);
                                    jsonWriter.WriteString("property_name", uuidToNameConverter.Convert(boardEvent.Uuid, null, null, System.Globalization.CultureInfo.InvariantCulture) as String);
                                    byte[] byteArray = ArrayPool<byte>.Shared.Rent(boardEvent.Data.Length);
                                    boardEvent.Data.CopyTo(byteArray, 0);
                                    jsonWriter.WriteString("raw_data", BitConverter.ToString(byteArray, 0, boardEvent.Data.Length));
                                    ArrayPool<byte>.Shared.Return(byteArray);
                                    jsonWriter.WriteEndObject();
                                }

                                jsonWriter.WriteEndArray();
                                jsonWriter.Flush();
                            }
                        }

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await PopupNavigation.Instance.PopAsync();

                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                taskCompletionSource.TrySetResult(false);
                                return;
                            }

                            await Share.RequestAsync(new ShareFileRequest
                            {
                                Title = Path.GetFileName(outputJSON),
                                File = new ShareFile(outputJSON)
                            });

                            taskCompletionSource.TrySetResult(true);
                        });
                    }
                    else if (exportFormat == csvButton)
                    {
                        var outputCSV = Path.Combine(Path.GetTempPath(), $"{baseFilename}.csv");

                        using (var streamWriter = new StreamWriter(outputCSV, false))
                        {
                            streamWriter.WriteLine("Timestamp,Property UUID,Property Name,Raw Data (hex)");

                            foreach (var boardEvent in model.BoardEvents)
                            {
                                if (cancellationTokenSource.IsCancellationRequested)
                                {
                                    break;
                                }

                                var propertyName = uuidToNameConverter.Convert(boardEvent.Uuid, null, null, System.Globalization.CultureInfo.InvariantCulture);
                                byte[] byteArray = ArrayPool<byte>.Shared.Rent(boardEvent.Data.Length);
                                boardEvent.Data.CopyTo(byteArray, 0);
                                streamWriter.WriteLine($"{boardEvent.Timestamp},{boardEvent.Uuid},\"{propertyName}\",{BitConverter.ToString(byteArray, 0, boardEvent.Data.Length)}");                                
                                ArrayPool<byte>.Shared.Return(byteArray);                                
                            }
                        }

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await PopupNavigation.Instance.PopAsync();


                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                taskCompletionSource.TrySetResult(false);
                                return;
                            }

                            await Share.RequestAsync(new ShareFileRequest
                            {
                                Title = Path.GetFileName(outputCSV),
                                File = new ShareFile(outputCSV)
                            });

                            taskCompletionSource.TrySetResult(true);
                        });
                    }
                }
            });

            return await taskCompletionSource.Task;
        }

        
	}
}

