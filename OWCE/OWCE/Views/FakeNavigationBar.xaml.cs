using System;
using System.Collections.Generic;
using OWCE.Pages;
using Xamarin.Forms;

namespace OWCE.Views
{
    public partial class FakeNavigationBar : Grid
    {

        public static readonly BindableProperty InnerContentProperty = BindableProperty.Create("InnerContent", typeof(View), typeof(FakeNavigationBar), null);
        public View InnerContent
        {
            get { return (View)GetValue(InnerContentProperty); }
            set { SetValue(InnerContentProperty, value); }
        }

        public FakeNavigationBar()
        {
            InitializeComponent();
            BindingContext = this;
        }


        /*
        public void BurgerMenu_Tapped(object sender, EventArgs e)
        {
            var parent = Parent;
            while (parent != null)
            {
                // TODO: ??
                
                //if (parent is MainFlyoutPage mainFlyoutPage)
                //{
                //    mainFlyoutPage.IsPresented = true;
                //    break;
                //}
                

                parent = parent.Parent;
            }
        }
        */
    }
}
