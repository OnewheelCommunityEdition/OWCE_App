using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
using OWCE.DependencyInterfaces;
using OWCE.Pages;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Linq;

[assembly: ExportFont("SairaExtraCondensed-Black.ttf")]
[assembly: ExportFont("SairaExtraCondensed-Bold.ttf")]
[assembly: ExportFont("SairaExtraCondensed-SemiBold.ttf")]
[assembly: ExportFont("SairaExtraCondensed-Light.ttf")]
[assembly: ExportFont("SairaExtraCondensed-Medium.ttf")]


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OWCE
{
    public partial class App : Application
    {
        public const string UnitDisplayUpdatedKey = "UnitDisplayUpdated";

        public static new App Current => Application.Current as App;
        public IOWBLE OWBLE { get; private set; }

#if DEBUG
        public const string OWCEApiServer = "api.dev.owce.app";
#else
        public const string OWCEApiServer = "api.owce.app";
#endif


        public static readonly BindableProperty MetricDisplayProperty = BindableProperty.Create(
            nameof(MetricDisplay),
            typeof(bool),
            typeof(App),
            false);

        public bool MetricDisplay
        {
            get { return (bool)GetValue(MetricDisplayProperty); }
            set { SetValue(MetricDisplayProperty, value); }
        }

        public string LogsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "past_rides");

        public App()
        {

            MetricDisplay = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);
            
            if (Directory.Exists(LogsDirectory) == false)
            {
                Directory.CreateDirectory(LogsDirectory);
            }

            Database.Init();

            InitializeComponent();

#if DEBUG
            // If simulator or emulator use MockOWBLE.
            if (DeviceInfo.DeviceType == DeviceType.Virtual)
            {
                /*
                var filenameRegex = new System.Text.RegularExpressions.Regex(@"^OWCE\.Resources\.SampleRideData\.(.*)\.bin$");
                var assembly = System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
                foreach (var resourceName in assembly.GetManifestResourceNames())
                {
                    var match = filenameRegex.Match(resourceName);
                    if (match.Success) //resourceName.StartsWith("OWCE.Resources.SampleRideData.")
                    {
                        var targetFilename = Path.Combine(LogsDirectory, match.Groups[1].Value + ".bin");
                        if (File.Exists(targetFilename) == false)
                        {
                            // TODO: Check filename? Check exists? Check checksum?
                            using (var fileStream = assembly.GetManifestResourceStream(resourceName))
                            {
                                using (var streamWriter = File.Create(targetFilename))
                                {
                                    fileStream.CopyTo(streamWriter);
                                }
                            }
                        }
                    }
                }
                */

                OWBLE = new MockOWBLE();
            }
            else
            {
                OWBLE = DependencyService.Get<IOWBLE>();
            }
#else
            OWBLE = DependencyService.Get<IOWBLE>();
#endif
            //MainPage = new MainFlyoutPage();
            MainPage = new CustomNavigationPage(new BoardListPage());
            //MainPage = new CustomNavigationPage(new SubmitRidePage(new Ride()));



            /*
            Debug.WriteLine("Before 1");
            Task.Run(async () =>
            {
                Debug.WriteLine("Before 2");
                await Task.Delay(1000);
                Debug.WriteLine("After 2");
            });
            Debug.WriteLine("After 1");
            */
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start($"android={AppConstants.AppCenterAndroid};ios={AppConstants.AppCenteriOS}", typeof(Analytics), typeof(Crashes));


            /*
            var cancellationTokenSource = new CancellationTokenSource();

            var file = Directory.GetFiles(App.Current.LogsDirectory, "*.bin").First();
            var rand = new Random();
            var baseBoard = new OWBaseBoard()
            {
                ID = "ow" + rand.Next(0, 999999).ToString("D6"),
                Name = Path.GetFileNameWithoutExtension(file),
                IsAvailable = true,
                NativePeripheral = file,
            };

           
            var board = await App.Current.ConnectToBoard(baseBoard, cancellationTokenSource.Token);
            if (board != null)
            {
                //MainPage = new NavigationPage(new TestPage());
                MainPage = new NavigationPage(new BoardPage(board)); // (new TestPage());
            }
            */
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        internal async Task<OWBoard> ConnectToBoard(OWBaseBoard baseBoard, CancellationToken token)
        {
            var didConnect = await OWBLE.Connect(baseBoard, token);
            if (didConnect)
            {
                return new OWBoard(OWBLE, baseBoard);
            }

            return null;
        }

        internal void DisconnectFromBoard()
        {
            /*
            OWBLE.Disconnect();
            OWBLE = null;
            */
        }
    }
}
