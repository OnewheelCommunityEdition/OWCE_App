using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWCE.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class AboutPage : BaseContentPage
    {
        public string VersionString => $"{AppInfo.VersionString} (build {AppInfo.BuildString})";

        IAsyncCommand _faqCommand;
        public IAsyncCommand FAQCommand => _faqCommand ??= new AsyncCommand(async () => await OpenUrlAsync("https://owce.app/faq/"), allowsMultipleExecutions: false);

        IAsyncCommand _sourceCodeCommand;
        public IAsyncCommand SourceCodeCommand => _sourceCodeCommand ??= new AsyncCommand(async () => await OpenUrlAsync("https://github.com/OnewheelCommunityEdition/OWCE_App"), allowsMultipleExecutions: false);

        IAsyncCommand _reportProblemCommand;
        public IAsyncCommand ReportProblemCommand => _reportProblemCommand ??= new AsyncCommand(async () => await OpenUrlAsync("https://github.com/OnewheelCommunityEdition/OWCE_App/issues/new/choose"), allowsMultipleExecutions: false);

        IAsyncCommand _requestFeatureCommand;
        public IAsyncCommand RequestFeatureCommand => _requestFeatureCommand ??= new AsyncCommand(async () => await OpenUrlAsync("https://github.com/OnewheelCommunityEdition/OWCE_App/issues/new/choose"), allowsMultipleExecutions: false);

        IAsyncCommand _redditCommand;
        public IAsyncCommand RedditCommand => _redditCommand ??= new AsyncCommand(async () => await OpenUrlAsync("https://reddit.com/r/OWCE"), allowsMultipleExecutions: false);

        IAsyncCommand _twitterCommand;
        public IAsyncCommand TwitterCommand => _twitterCommand ??= new AsyncCommand(async () => await OpenUrlAsync("https://twitter.com/owceapp"), allowsMultipleExecutions: false);

        IAsyncCommand _facebookPageCommand;
        public IAsyncCommand FacebookPageCommand => _facebookPageCommand ??= new AsyncCommand(async () => await OpenUrlAsync("https://www.facebook.com/owceapp"), allowsMultipleExecutions: false);

        IAsyncCommand _facebookGroupCommand;
        public IAsyncCommand FacebookGroupCommand => _facebookGroupCommand ??= new AsyncCommand(async () => await OpenUrlAsync("https://www.facebook.com/groups/owceappgroup"), allowsMultipleExecutions: false);
                  


        public AboutPage()
        {
            InitializeComponent();

            CustomToolbarItems.Add(new Views.CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Left,
                Text = "Cancel",
                Command = new AsyncCommand(async () =>
                {
                    await Navigation.PopModalAsync();
                }, allowsMultipleExecutions: false),
            });

            BindingContext = this;
        }


        async Task OpenUrlAsync(string url)
        {
            // Try launch browser, but if we can't launch internal browser.
            if (await Launcher.TryOpenAsync(url) == false)
            {
                await Browser.OpenAsync(url);
            }
        }
    }
}
