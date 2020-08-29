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

        UIBarButtonItem _masterDetailPageButton;

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);


            var leftBarButton = this.NavigationItem.LeftBarButtonItems;
            return;
            var firstViewController = ViewControllers.FirstOrDefault();
            if (_masterDetailPageButton == null && firstViewController.NavigationItem.LeftBarButtonItem.Image.AccessibilityIdentifier == "hamburger")
            {
                /*
                firstViewController.NavigationItem.LeftBarButtonItem.CustomView = new UIView(new CGRect(0, 0, 44, 44))
                {
                    UserInteractionEnabled = false,
                    BackgroundColor = UIColor.Red,
                };
                */
                _masterDetailPageButton = firstViewController.NavigationItem.LeftBarButtonItem;

                var image = UIImage.FromBundle("burger_menu").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);

                var newButton = UIButton.FromType(UIButtonType.Custom);
                newButton.Frame = new CGRect(0, 0, 28, 28);
                newButton.TouchUpInside += NewButton_TouchUpInside;


                newButton.ContentMode = newButton.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                newButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                newButton.VerticalAlignment = UIControlContentVerticalAlignment.Center;
                newButton.SetImage(image, UIControlState.Normal);
                var barButtonItem = new UIBarButtonItem(newButton);

                var currentWidth = barButtonItem.CustomView?.WidthAnchor.ConstraintEqualTo(24);
                currentWidth.Active = false;

                var currentHeight = barButtonItem.CustomView?.HeightAnchor.ConstraintEqualTo(24);
                currentHeight.Active = false;

                firstViewController.NavigationItem.LeftBarButtonItem = barButtonItem;
                // = firstViewController.NavigationItem.LeftBarButtonItem.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                //= 

            }

            /*
        var masterDetailPage = Element as MasterDetailPage;
        if (!(masterDetailPage?.Detail is NavigationPage))
            return;

        var detailRenderer = Platform.GetRenderer(masterDetailPage.Detail) as UINavigationController;

        UIViewController firstPage = detailRenderer?.ViewControllers.FirstOrDefault();
        if (firstPage != null)
            NavigationRenderer.SetMasterLeftBarButton(firstPage, masterDetailPage);
            */

        }

        private void NewButton_TouchUpInside(object sender, EventArgs e)
        {
            if (_masterDetailPageButton != null)
            {
                UIApplication.SharedApplication.SendAction(_masterDetailPageButton.Action, _masterDetailPageButton.Target, null, null);

               // _masterDetailPageButton.Target.PerformSelector(_masterDetailPageButton.Action);
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
