using System;
using OWCE.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class PastRidesPage : BaseContentPage
    {
        public ObservableRangeCollection<Ride> Rides { get; set; } = new ObservableRangeCollection<Ride>();

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

            var rides = Database.Connection.Table<Ride>().OrderByDescending((r) => r.StartTime);
            Rides.AddRange(rides);

            BindingContext = this;
        }

        bool _hasFirstLoad = false;
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_hasFirstLoad == false)
            {
                _hasFirstLoad = true;

            }
        }
    }
}
