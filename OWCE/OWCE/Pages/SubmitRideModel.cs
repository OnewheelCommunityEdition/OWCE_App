using System;
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
    }
}

