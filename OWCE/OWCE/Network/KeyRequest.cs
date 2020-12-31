using System;
using System.Text.Json.Serialization;

namespace OWCE.Network
{
    public class KeyRequest
    {
        [JsonPropertyName("device_name")]
        public string DeviceName { get; set; }

        [JsonPropertyName("ow_type")]
        public string OWType { get; set; }

        [JsonPropertyName("api_key")]
        public string APIKey { get; set; }

        [JsonPropertyName("board_key")]
        public string BoardKey { get; set; }
    }
}
