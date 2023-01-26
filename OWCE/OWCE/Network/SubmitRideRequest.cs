using System;
using System.Text.Json.Serialization;

namespace OWCE.Network
{
    public class SubmitRideRequest
    {
        [JsonPropertyName("ride_name")]
        public string RideName { get; set; } = String.Empty;
        
        [JsonPropertyName("aftermarket_battery")]
        public bool AftermarketBattery { get; set; } = false;

        [JsonPropertyName("battery_type")]
        public string BatteryType { get; set; } = String.Empty;

        [JsonPropertyName("remove_identifiers")]
        public bool RemoveIdentifiers { get; set; } = false;

        [JsonPropertyName("allow_publicly")]
        public bool AllowPublicly { get; set; } = false;

        [JsonPropertyName("additional_notes")]
        public string AdditionalNotes { get; set; } = String.Empty;
    }
}

