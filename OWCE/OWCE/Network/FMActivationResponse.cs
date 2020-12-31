using System;
using System.Text.Json.Serialization;

namespace OWCE.Network
{
    public class FMActivationResponse
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
