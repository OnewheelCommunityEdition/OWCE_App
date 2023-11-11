using System;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace OWCE.Pages.Popup
{
    public partial class BoadTokenPopup : PopupPage
    {
        private readonly Command _saveButtonCommand;

        public String BoardKey { get; set; }

        public String BoardName { get; set; }

        private string _BoardToken;
        public string BoardToken
        {
            get => _BoardToken;
            set
            {
                _BoardToken = value;
                // Only token is required, OWCE will read the key durring the handshake and save it for the next time.
                IsSaveButtonEnabled = !String.IsNullOrEmpty(value);
                this.OnPropertyChanged(nameof(BoardToken));
            }
        }


        private bool _IsSaveButtonEnabled;
        public bool IsSaveButtonEnabled
        {
            get => _IsSaveButtonEnabled;
            set
            {
                _IsSaveButtonEnabled = value;
                this.OnPropertyChanged(nameof(IsSaveButtonEnabled));
            }
        }

        public Command SaveButtonCommand => _saveButtonCommand;

        public BoadTokenPopup(OWBaseBoard board, String key, String token)
        {
            BoardKey = key;
            BoardToken = token;
            BoardName = board.Name;

            BindingContext = this;
            _saveButtonCommand = new Command(async () =>
            {
                if (String.IsNullOrWhiteSpace(BoardKey))
                {
                    BoardSecureStorage.RemoveBoardKey(BoardName);
                }
                else
                {
                    await BoardSecureStorage.SetBoardKeyAsync(BoardName, BoardKey.Trim());
                }

                if (String.IsNullOrWhiteSpace(BoardToken))
                {
                    BoardSecureStorage.RemoveBoardToken(BoardName);
                }
                else
                {
                    await BoardSecureStorage.SetBoardTokenAsync(BoardName, BoardToken.Trim());
                }

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.RemovePageAsync(this);
            });

            InitializeComponent();
        }
    }
}
