using System;
using Xamarin.Forms;

namespace OWCE
{
    public static class Hud
    {
        public static void Show(string message)
        {
            DependencyService.Get<IHud>().Show(message);
        }

        public static void Show(string message, float progress)
        {
            DependencyService.Get<IHud>().Show(message, progress);
        }

        public static void Show(string message, float progress, string cancel, Action cancelCallback)
        {
            DependencyService.Get<IHud>().Show(message, progress, cancel, cancelCallback);
        }

        public static void Show(string message, string cancel, Action cancelCallback)
        {
            DependencyService.Get<IHud>().Show(message, cancel, cancelCallback);
        }

        public static void Dismiss()
        {
            DependencyService.Get<IHud>().Dismiss();
        }
    }
}
