using System;
using OWCE.iOS.Renderers;
using OWCE.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MainMasterPage), typeof(MainMasterPageRenderer))]

namespace OWCE.iOS.Renderers
{
    public class MainMasterPageRenderer : PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
    }
}
