using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace OWCE.Views
{
    public partial class FootpadsView : ContentView
    {
        public static readonly BindableProperty LeftPadEngagedProperty =  BindableProperty.Create(
            "LeftPadEngaged",
            typeof(bool),
            typeof(FootpadsView),
            false);

        public bool LeftPadEngaged
        {
            get { return (bool)GetValue(LeftPadEngagedProperty); }
            set { SetValue(LeftPadEngagedProperty, value); }
        }

        public FootpadsView()
        {
            InitializeComponent();
        }
    }
}
