using System;
using System.Linq;
using CoreGraphics;
using OWCE.iOS.Renderers;
using OWCE.Pages;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomNavigationPage), typeof(CustomNavigationPageRenderer))]

namespace OWCE.iOS.Renderers
{
    public class CustomNavigationPageRenderer : NavigationRenderer
    {


        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= PropertyChanged;
                e.OldElement.PropertyChanging -= PropertyChanging;
            }

            if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += PropertyChanged;
                e.NewElement.PropertyChanging += PropertyChanging;
            }
        }

        private void PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            //Console.WriteLine($"PropertyChanging: {e.PropertyName}");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Console.WriteLine($"PropertyChanged: {e.PropertyName}");
            if (NavigationPage.BarBackgroundColorProperty.PropertyName.Equals(e.PropertyName))
            {
                if (Element is Xamarin.Forms.NavigationPage navigationPage)
                {
                    NavigationBar.BarTintColor = navigationPage.BarBackgroundColor.ToUIColor();
                }
            }
            else if (NavigationPage.IconImageSourceProperty.PropertyName.Equals(e.PropertyName))
            {
                Console.WriteLine("X");
            }
        }
            
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (Element is Xamarin.Forms.NavigationPage navigationPage)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                {
                    NavigationBar.CompactAppearance.ShadowColor = UIColor.Clear;
                    NavigationBar.StandardAppearance.ShadowColor = UIColor.Clear;
                    NavigationBar.ScrollEdgeAppearance.ShadowColor = UIColor.Clear;
                }

                if (navigationPage.BarBackgroundColor == Color.Default)
                {
                    NavigationBar.BarTintColor = ((Color)App.Current.Resources["BackgroundGradientStart"]).ToUIColor();
                }
                else
                {
                    NavigationBar.BarTintColor = navigationPage.BarBackgroundColor.ToUIColor();
                }
            }
        }
    }
}
