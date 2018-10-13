using System;
using System.Collections.Generic;
using Plugin.BLE;
using Xamarin.Essentials;
using Xamarin.Forms;


#if __IOS__
using Foundation;

#endif

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

        public BoardPage(OWBoard board)
        {
            Board = board;
            board.SubscribeToBLE();

            BindingContext = this;

            InitializeComponent();

            NavigationPage.SetHasBackButton(this, false);

        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (GaugeAbsolueLayout.WidthRequest != width)
            {
                GaugeAbsolueLayout.WidthRequest = width;
                GaugeAbsolueLayout.HeightRequest = width;
                GaugeAbsolueLayout.MinimumWidthRequest = width;
                GaugeAbsolueLayout.MinimumHeightRequest = width;
            }
        }

        async void Disconnect_Clicked(object sender, System.EventArgs e)
        {
            await Board.Disconnect();
            await Navigation.PopAsync();
        }

        private bool _isLogging = false;

        private async void LogData_Clicked(object sender, System.EventArgs e)
        {
            if (_isLogging)
            {
                LogDataButton.Text = "Start Logging Data";
                _isLogging = false;
                string zip = await Board.StopLogging();
                Hud.Dismiss();

#if __IOS__
                NSUrl item = NSUrl.FromFilename(zip);
                var controller = new UIKit.UIActivityViewController(new NSObject[] { item }, null);
                controller.ExcludedActivityTypes = new Foundation.NSString[] {
                    UIKit.UIActivityType.AddToReadingList,
                    //UIKit.UIActivityType.AirDrop,
                    UIKit.UIActivityType.AssignToContact,
                    UIKit.UIActivityType.CopyToPasteboard,
                    UIKit.UIActivityType.Mail,
                    UIKit.UIActivityType.MarkupAsPdf,
                    UIKit.UIActivityType.Message,
                    UIKit.UIActivityType.OpenInIBooks,
                    UIKit.UIActivityType.PostToFacebook,
                    UIKit.UIActivityType.PostToFlickr,
                    UIKit.UIActivityType.PostToTencentWeibo,
                    UIKit.UIActivityType.PostToTwitter,
                    UIKit.UIActivityType.PostToVimeo,
                    UIKit.UIActivityType.PostToWeibo,
                    UIKit.UIActivityType.Print,
                    UIKit.UIActivityType.SaveToCameraRoll
                };
                UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(controller, true, null);
#elif __ANDROID__
                var shareIntent = new Android.Content.Intent(Android.Content.Intent.ActionSend);
                shareIntent.SetType("application/zip");
                var dirName = System.IO.Path.GetDirectoryName(zip);
                var zipName = System.IO.Path.GetFileName(zip);

                var uri = Android.Net.Uri.FromFile(new Java.IO.File(dirName, zipName));
                shareIntent.PutExtra(Android.Content.Intent.ExtraStream, uri);
#endif


            }
            else
            {
                LogDataButton.Text = "Stop Logging Data";
                _isLogging = true;
                Board.StartLogging();

            }

        }

        }
}
