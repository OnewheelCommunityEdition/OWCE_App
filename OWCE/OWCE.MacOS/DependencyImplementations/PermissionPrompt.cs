using System;
using System.Threading.Tasks;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.MacOS.DependencyImplementations.PermissionPrompt))]

namespace OWCE.MacOS.DependencyImplementations
{
    public class PermissionPrompt : IPermissionPrompt
    {
        public async Task<bool> PromptBLEPermission()
        {
            return true;
        }
    }
}
