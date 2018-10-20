using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OWCE
{
    public partial class MyRidesPage : ContentPage
    {
        public ObservableCollection<Ride> Rides { get; internal set; } = new ObservableCollection<Ride>();

        public MyRidesPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Rides = new ObservableCollection<Ride>();

            if (Directory.Exists(FileSystem.CacheDirectory))
            {
                string[] directories = Directory.GetDirectories(FileSystem.CacheDirectory);

                foreach (string directory in directories)
                {
                    var dirName = Path.GetFileName(directory);

                    // Gross.
                    if (dirName.Length == 10 && dirName.StartsWith("15"))
                    {
                    //    "/var/mobile/Containers/Data/Application/E1333911-5A43-4E8D-BB91-703A6C93221E/Library/Caches/"
                        long tempLong = 0;
                        if (long.TryParse(dirName, out tempLong))
                        {
                            var offest = DateTimeOffset.FromUnixTimeSeconds(tempLong);
                            Rides.Add(new Ride(offest.DateTime, directory));
                        }
                    }
                }
            }

            this.MyRidesListView.ItemsSource = null;
            this.MyRidesListView.ItemsSource = Rides;

        }
    }
}
