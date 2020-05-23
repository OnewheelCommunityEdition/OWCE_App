using System;
using System.Threading.Tasks;
using OWCE.DependencyInterfaces;
using Plugin.Permissions;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.Droid.DependencyImplementations.PermissionPrompt))]

namespace OWCE.Droid.DependencyImplementations
{
    public class PermissionPrompt : IPermissionPrompt
    {
        public async Task<bool> PromptBLEPermission()
        {
            if ((int)Android.OS.Build.VERSION.SdkInt >= 23)
            {
                var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (permissionStatus != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert("Oops", "In order to for bluetooth to scan for your board you need to enable location permission.\n(Yeah, that is as confusing as it sounds)", "Ok");

                    permissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();                    
                }

                if (permissionStatus == PermissionStatus.Denied)
                {
                    var shouldOpenSettings = await Application.Current.MainPage.DisplayAlert("Error", "In order to for bluetooth to scan for your board you need to enable location permission.\n(Yeah, that is as confusing as it sounds)", "Open Settings", "Cancel");
                    if (shouldOpenSettings)
                    {
                        AppInfo.ShowSettingsUI();
                    }
                    return false;
                }
            }

            return true;
        }
    }
}
