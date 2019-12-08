using System;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(OWCE.MacOS.DependencyImplementations.Hud))]

namespace OWCE.MacOS.DependencyImplementations
{
    public class Hud : IHud
    {
        public void Dismiss()
        {
            throw new NotImplementedException();
        }

        public void Show(string message)
        {
            throw new NotImplementedException();
        }

        public void Show(string message, float progress)
        {
            throw new NotImplementedException();
        }

        public void Show(string message, float progress, string cancel, Action cancelCallback)
        {
            throw new NotImplementedException();
        }

        public void Show(string message, string cancel, Action cancelCallback)
        {
            throw new NotImplementedException();
        }
    }
}
