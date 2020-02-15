using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
using OWCE.DependencyInterfaces;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OWCE
{
    public partial class App : Application
    {
        public static new App Current => Application.Current as App;
        public IOWBLE OWBLE { get; private set; }

        public bool MetricDisplay
        {
            get; set;
        }

        public bool SpeedDemon
        {
            get; set;
        }

        public App()
        {
            MetricDisplay = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);
            SpeedDemon = Preferences.Get("speed_demon", false);


            OWBLE = DependencyService.Get<IOWBLE>();

            if (String.IsNullOrEmpty(AppConstants.SyncfusionLicense) == false)
            {
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(AppConstants.SyncfusionLicense);
            }
            InitializeComponent();

            bool showNote = false;
            bool neverRemindMe = Preferences.Get("third_party_remind_me_never", false);

            if (VersionTracking.IsFirstLaunchForCurrentBuild)
            {
                Preferences.Set("third_party_seen_message_on_this_version", false);
            }


            if (neverRemindMe == false && VersionTracking.IsFirstLaunchForCurrentBuild)
            {
                bool seenMessageOnThisVersion = Preferences.Get("third_party_seen_message_on_this_version", false);
                if (seenMessageOnThisVersion == false)
                {
                    showNote = true;
                }
            }

            if (showNote)
            {
                MainPage = new NavigationPage(new Pages.ThirdPartyNotePage());
            }
            else
            {
                MainPage = new NavigationPage(new BoardListPage());
            }

            //MainPage = new MainMasterDetailPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start($"android={AppConstants.AppCenterAndroid};ios={AppConstants.AppCenteriOS}", typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
