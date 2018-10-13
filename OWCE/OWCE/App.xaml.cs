using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OWCE
{
    public partial class App : Application
    {
        public App()
        {
            if (String.IsNullOrEmpty(AppConstants.SyncfusionLicense) == false)
            {
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(AppConstants.SyncfusionLicense);
            }
            InitializeComponent();

            /*
            var owBoard = new OWBoard();
            owBoard.BatteryPercent = 96;
            owBoard.RPM = 22;
            Preferences.Set("speed_demon", true);
            */
            //MainPage = new NavigationPage(new BoardPage(owBoard)); 
            MainPage = new MainMasterDetailPage();
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
