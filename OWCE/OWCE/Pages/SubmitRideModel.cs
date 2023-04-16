using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OWCE.Network;
using Xamarin.CommunityToolkit.ObjectModel;

namespace OWCE.Pages
{
    public class SubmitRideModel : Xamarin.CommunityToolkit.ObjectModel.ObservableObject
    {
        string rideName = String.Empty;
        public string RideName
        {
            get => rideName;
            set => SetProperty(ref rideName, value);
        }

        bool isAftermarketBattery = false;
        public bool IsAftermarketBattery
        {
            get => isAftermarketBattery;
            set => SetProperty(ref isAftermarketBattery, value);
        }

        string batteryType = String.Empty;
        public string BatteryType
        {
            get => batteryType;
            set => SetProperty(ref batteryType, value);
        }

        bool removeIdentifiers = true;
        public bool RemoveIdentifiers
        {
            get => removeIdentifiers;
            set => SetProperty(ref removeIdentifiers, value);
        }

        bool allowPublicly = false;
        public bool AllowPublicly
        {
            get => allowPublicly;
            set => SetProperty(ref allowPublicly, value);
        }

        string additionalNotes = String.Empty;
        public string AdditionalNotes
        {
            get => additionalNotes;
            set => SetProperty(ref additionalNotes, value);
        }

        AsyncRelayCommand _viewDataSubmittedCommand;
        public AsyncRelayCommand ViewDataSubmittedCommand => _viewDataSubmittedCommand ??= new AsyncRelayCommand(ViewDataSubmittedAsync);

        WeakReference<SubmitRidePage> _page = null;

        public SubmitRideModel(SubmitRidePage page)
        {
            _page = new WeakReference<SubmitRidePage>(page);
        }

        async Task ViewDataSubmittedAsync()
        {
            if (_page.TryGetTarget(out SubmitRidePage page))
            {
                await page.ViewDataSubmittedAsync();
            }
        }

        internal SubmitRideRequest GetSubmitRideRequest()
        {
            return new SubmitRideRequest()
            {
                RideName = RideName,
                AftermarketBattery = IsAftermarketBattery,
                BatteryType = BatteryType,
                RemoveIdentifiers = RemoveIdentifiers,
                AllowPublicly = AllowPublicly,
                AdditionalNotes = AdditionalNotes,
            };
        }
    }
}

