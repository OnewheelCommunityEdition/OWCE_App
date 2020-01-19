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
                var locationPermission = Plugin.Permissions.Abstractions.Permission.Location;
                var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(locationPermission);


                if (permissionStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert("Oops", "In order to for bluetooth to scan for your board you need to enable location permission.\n(Yeah, that is as confusing as it sounds)", "Ok");


                    var result = await CrossPermissions.Current.RequestPermissionsAsync(locationPermission);
                    
                    permissionStatus = result[locationPermission];
                }

                if (permissionStatus == Plugin.Permissions.Abstractions.PermissionStatus.Denied)
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
