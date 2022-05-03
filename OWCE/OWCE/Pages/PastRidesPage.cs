using System;
using System.Collections.Generic;
using OWCE.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class PastRidesPage : BaseContentPage
    {
        public PastRidesPage()
        {
            InitializeComponent();


            CustomToolbarItems.Add(new Views.CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Left,
                Text = "Close",
                Command = new AsyncCommand(async () =>
                {
                    await Navigation.PopModalAsync();
                }, allowsMultipleExecutions: false),
            });

            BindingContext = this;
        }
    }
}
