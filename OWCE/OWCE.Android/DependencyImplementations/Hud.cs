using System;
using AndroidHUD;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.Droid.DependencyImplementations.Hud))]

namespace OWCE.Droid.DependencyImplementations
{
    public class Hud : IHud
    {
        MaskType _maskType = MaskType.Black;

        public void Show(string message)
        {
            AndHUD.Shared.Show(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, message, maskType: _maskType);
        }

        public void Show(string message, float progress)
        {
            AndHUD.Shared.Show(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, message, (int)(progress * 100), _maskType);
        }

        public void Show(string message, float progress, string cancel, Action cancelCallback)
        {
            AndHUD.Shared.Show(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, message, (int)(progress * 100), _maskType, null, null, true, cancelCallback);
        }

        public void Show(string message, string cancel, Action cancelCallback)
        {
            AndHUD.Shared.Show(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, message, -1, _maskType, null, null, true, cancelCallback);
        }

        public void Dismiss()
        {
            AndHUD.Shared.Dismiss(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity);
        }
    }
}
