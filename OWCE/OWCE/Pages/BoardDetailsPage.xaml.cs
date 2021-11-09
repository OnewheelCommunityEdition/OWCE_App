using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class BoardDetailsPage : ContentPage
    {
        public BoardDetailsPage(OWBoard board)
        {
            BindingContext = board;

            InitializeComponent();

            ToolbarItems.Add(new ToolbarItem("Cancel", null, () =>
            {
                Navigation.PopModalAsync();
            }));
        }
    }
}
