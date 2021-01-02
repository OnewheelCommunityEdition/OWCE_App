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


//SairaExtraCondensed-SemiBold
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OWCE
{
    public partial class App : Application
    {
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

        public static readonly BindableProperty SpeedReportingProperty = BindableProperty.Create(
            nameof(SpeedReporting),
            typeof(bool),
            typeof(App),
            false);

        public static readonly BindableProperty SpeedReportingBaselineTimeoutProperty = BindableProperty.Create(
            nameof(SpeedReportingBaselineTimeout),
            typeof(int),
            typeof(App),
            15);

        public static readonly BindableProperty SpeedReportingMinimumProperty = BindableProperty.Create(
            nameof(SpeedReportingMinimum),
            typeof(int),
            typeof(App),
            10);

        public static readonly BindableProperty BatteryPercentReportingProperty = BindableProperty.Create(
            nameof(BatteryPercentReporting),
            typeof(bool),
            typeof(App),
            false);

        public static readonly BindableProperty BatteryPercentInferredBasedOnVoltageProperty = BindableProperty.Create(
            nameof(BatteryPercentInferredBasedOnVoltage),
            typeof(bool),
            typeof(App),
            false);

        public bool MetricDisplay
        {
            get { return (bool)GetValue(MetricDisplayProperty); }
            set { SetValue(MetricDisplayProperty, value); }
        }

        public bool SpeedReporting
        {
            get { return (bool)GetValue(SpeedReportingProperty); }
            set { SetValue(SpeedReportingProperty, value); }
        }

        public int SpeedReportingBaselineTimeout
        {
            get { return (int)GetValue(SpeedReportingBaselineTimeoutProperty); }
            set { SetValue(SpeedReportingBaselineTimeoutProperty, value); }
        }

        public int SpeedReportingMinimum
        {
            get { return (int)GetValue(SpeedReportingMinimumProperty); }
            set { SetValue(SpeedReportingMinimumProperty, value); }
        }

        public bool BatteryPercentReporting
        {
            get { return (bool)GetValue(BatteryPercentReportingProperty); }
            set { SetValue(BatteryPercentReportingProperty, value); }
        }

        public bool BatteryPercentInferredBasedOnVoltage
        {
            get { return (bool)GetValue(BatteryPercentInferredBasedOnVoltageProperty); }
            set { SetValue(BatteryPercentInferredBasedOnVoltageProperty, value); }
        }

        public string LogsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "beta_ride_logs");

        public App()
        {
            Xamarin.Forms.Device.SetFlags(new string[] { "Shapes_Experimental", "Expander_Experimental" });

            MetricDisplay = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);
            SpeedReporting = Preferences.Get("speed_reporting", false);
            SpeedReportingBaselineTimeout = Preferences.Get("speedreporting_baseline_timeout", 15);
            SpeedReportingMinimum = Preferences.Get("speedreporting_minimum", System.Globalization.RegionInfo.CurrentRegion.IsMetric ? 15 : 10);
            BatteryPercentReporting = Preferences.Get("batterypercent_reporting", false);
            BatteryPercentInferredBasedOnVoltage = Preferences.Get("batterypercent_inferred_voltage", false);
            
            if (Directory.Exists(LogsDirectory) == false)
            {
                Directory.CreateDirectory(LogsDirectory);
            }

            InitializeComponent();

#if DEBUG
            // If simulator or emulator use MockOWBLE.
            if (DeviceInfo.DeviceType == DeviceType.Virtual)
            {
                OWBLE = new MockOWBLE();
            }
            else
            {
                OWBLE = DependencyService.Get<IOWBLE>();
            }
#else
            OWBLE = DependencyService.Get<IOWBLE>();
#endif
            //MainPage = new MainMasterDetailPage();
            MainPage = new NavigationPage(new BoardListPage());



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
