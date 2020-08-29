using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace OWCE.Pages
{
    public class CustomNavigationPage : Xamarin.Forms.NavigationPage
    {
        public CustomNavigationPage(Xamarin.Forms.Page root) : base(root)
        {
            On<iOS>().SetHideNavigationBarSeparator(true);

        }

        protected override void OnParentSet()
        {
            base.OnParentSet();


            //BarBackgroundColor = App.Current.Resources[""];
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            /*
            var oldBarBackgroundColor = BarBackgroundColor;
            BarBackgroundColor = Color.Beige;
            BarBackgroundColor = oldBarBackgroundColor;
            */
        }
    }
}
