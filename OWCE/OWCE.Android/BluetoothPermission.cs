using System;
using System.Collections.Generic;

namespace OWCE.Droid
{
    internal class BluetoothPermission : Xamarin.Essentials.Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
            (Android.Manifest.Permission.BluetoothScan, true),
            (Android.Manifest.Permission.BluetoothConnect, true)
        }.ToArray();
    }
}

