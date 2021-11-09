using System;
using System.Text.Json.Serialization;

namespace OWCE.Network
{
    public class KeyResponse
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
