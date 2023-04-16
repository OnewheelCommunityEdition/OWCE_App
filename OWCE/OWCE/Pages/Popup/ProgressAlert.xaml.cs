using System;
using System.Collections.Generic;
using System.Windows.Input;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace OWCE.Pages.Popup
{
    public partial class ProgressAlert : PopupPage
    {
        public string SuperTitleText { get; set; } = String.Empty;
        public string TitleText { get; set; } = String.Empty;
        public string ConnectingText { get; set; } = String.Empty;
        public string ButtonText { get; set; } = "Cancel";

        private readonly ICommand _actionButtonCommand;
        public ICommand ActionButtonCommand => _actionButtonCommand;

        public ProgressAlert(ICommand cancelCommand, string titleText = null)
        {
            BindingContext = this;

            _actionButtonCommand = cancelCommand;

            TitleText = titleText ?? "Uploading";
            //ConnectingText = connectingText ?? "Connecting...";
            InitializeComponent();
        }
    }
}

