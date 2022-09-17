using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OWCE.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace OWCE.Pages
{
    public partial class PastRidesPage : BaseContentPage
    {
        public ObservableRangeCollection<Ride> Rides { get; set; } = new ObservableRangeCollection<Ride>();

        //AsyncCommand<Ride> rideSelectedCommand;
        //public AsyncCommand<Ride> RideSelectedCommand => rideSelectedCommand ??= new AsyncCommand<Ride>(RideSelected, allowsMultipleExecutions: false);

        /*
        AsyncCommand<Ride> deleteRideCommand;
        public AsyncCommand<Ride> DeleteRideCommand => deleteRideCommand ??= new AsyncCommand<Ride>(RideSelected, allowsMultipleExecutions: false);

        AsyncCommand<Ride> renameRideCommand;
        public AsyncCommand<Ride> RenameRideCommand => renameRideCommand ??= new AsyncCommand<Ride>(RideSelected, allowsMultipleExecutions: false);
        */

        //DeleteRideCommand
        //RenameRideCommand

        public PastRidesPage()
        {
            InitializeComponent();


            CustomToolbarItems.Add(new Views.CustomToolbarItem()
            {
                Position = CustomToolbarItemPosition.Left,
                Text = "Close",
                Command = new AsyncCommand(async () =>
                {
                    await Navigation.PopAsync();
                }, allowsMultipleExecutions: false),
            });

            // TODO: ??
            //var rides = Database.Connection.Table<Ride>().OrderByDescending((r) => r.StartTime);
            //Rides.AddRange(rides);

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

        void CollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
        }


        [RelayCommand]
        async Task RideSelected(Ride ride)
        {
            await DisplayAlert("Sorry", "Viewing of recorded ride is not implemented yet. However if you would like to support OWCE you can submit your ride to be used to further develop the app.", "OKAY");

        }


        [RelayCommand]
        async Task DeleteRide(Ride ride)
        {
            var delete = await DisplayAlert("Delete Ride", $"Are you sure you want to delete the ride \"{ride.Name}\"?", "Delete", "Cancel");
            if (delete)
            {
                Database.Connection.Delete(ride);
                Rides.Remove(ride);
            }
        }

        [RelayCommand]
        async Task RenameRide(Ride ride)
        {
            var name = await DisplayPromptAsync("Rename Ride", "What would like to rename this ride to?", "Save", initialValue: ride.Name);
            if (String.IsNullOrWhiteSpace(name) == false)
            {
                ride.Name = name;
                ride.Save();
            }
            else if (name == null)
            {
                // NOOP
            }
            else
            {
                await DisplayAlert("Error", $"\"{name}\" was an invalid name for a ride.", "OKAY");
            }
        }

    }
}
