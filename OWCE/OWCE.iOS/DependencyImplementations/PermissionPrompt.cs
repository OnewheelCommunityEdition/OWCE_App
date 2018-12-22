using System;
using System.Threading.Tasks;
using OWCE.DependencyInterfaces;
using Plugin.Permissions;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.iOS.DependencyImplementations.PermissionPrompt))]

namespace OWCE.iOS.DependencyImplementations
{
    public class PermissionPrompt : IPermissionPrompt
    {
        public async Task<bool> PromptBLEPermission()
        {
            return true;
        }
    }
}
