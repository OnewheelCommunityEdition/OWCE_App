using System;
using OWCE.iOS.Renderers;
using OWCE.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MainMasterDetailPage), typeof(MainMasterDetailPageRenderer))]

namespace OWCE.iOS.Renderers
{
    public class MainMasterDetailPageRenderer : PhoneMasterDetailRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {

            }

            if (e.NewElement != null)
            {
            }
        }
    }
}
