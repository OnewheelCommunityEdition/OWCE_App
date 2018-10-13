using System;
using AndroidHUD;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.Droid.Hud))]

namespace OWCE.Droid
{
    public class Hud : IHud
    {
        MaskType _maskType = MaskType.Black;

        public void Show(string message)
        {
            AndHUD.Shared.Show(Android.App.Application.Context, message, maskType: _maskType);
        }

        public void Show(string message, float progress)
        {
            AndHUD.Shared.Show(Android.App.Application.Context, message, (int)(progress * 100), _maskType);
        }

        public void Show(string message, float progress, string cancel, Action cancelCallback)
        {
            AndHUD.Shared.Show(Android.App.Application.Context, message, (int)(progress * 100), _maskType, null, null, true, cancelCallback);
        }

        public void Show(string message, string cancel, Action cancelCallback)
        {
            AndHUD.Shared.Show(Android.App.Application.Context, message, -1, _maskType, null, null, true, cancelCallback);
        }

        public void Dismiss()
        {
            AndHUD.Shared.Dismiss(Android.App.Application.Context);
        }
    }
}
