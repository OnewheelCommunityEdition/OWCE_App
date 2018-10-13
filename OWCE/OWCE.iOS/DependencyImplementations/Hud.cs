using System;
using BigTed;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.iOS.Hud))]

namespace OWCE.iOS
{
    public class Hud : IHud
    {
        public void Show(string message)
        {
            BTProgressHUD.Show(message, -1, ProgressHUD.MaskType.Gradient);
        }

        public void Show(string message, float progress)
        {
            BTProgressHUD.Show(message, progress, ProgressHUD.MaskType.Gradient);
        }

        public void Show(string message, float progress, string cancel, Action cancelCallback)
        {
            BTProgressHUD.Show(cancel, cancelCallback, message, progress, ProgressHUD.MaskType.Gradient);
        }

        public void Show(string message, string cancel, Action cancelCallback)
        {
            BTProgressHUD.Show(cancel, cancelCallback, message, -1, ProgressHUD.MaskType.Gradient);
        }

        public void Dismiss()
        {
            BTProgressHUD.Dismiss();
        }
    }
}
