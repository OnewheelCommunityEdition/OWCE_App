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
            // Android 12 and higher gets new permissions
            if ((int)Android.OS.Build.VERSION.SdkInt >= 31)
            {
                var permissionStatus = await Permissions.CheckStatusAsync<BluetoothPermission>();

                if (permissionStatus == PermissionStatus.Granted)
                {
                    return true;
                }


                permissionStatus = await Permissions.RequestAsync<BluetoothPermission>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    return true;
                }

                // Something didn't go right. Direct user to settings to hopefully enable permission.
                var shouldOpenSettings = await Application.Current.MainPage.DisplayAlert("Error", "In order to connect to your Onewheel you need to enable bluetooth permissions.", "Open Settings", "Cancel");
                if (shouldOpenSettings)
                {
                    AppInfo.ShowSettingsUI();
                }
                return false;
            }
            else // Android 11 and lower gets the old permissions.
            {
                var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    return true;
                }

                await Application.Current.MainPage.DisplayAlert("Oops", "In order to for bluetooth to scan for your board you need to enable location permission.\n(Yeah, that is as confusing as it sounds)", "Ok");

                permissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    return true;
                }

                var shouldOpenSettings = await Application.Current.MainPage.DisplayAlert("Error", "In order to for bluetooth to scan for your board you need to enable location permission.\n(Yeah, that is as confusing as it sounds)", "Open Settings", "Cancel");
                if (shouldOpenSettings)
                {
                    AppInfo.ShowSettingsUI();
                }
                return false;
            }
        }
    }
}
