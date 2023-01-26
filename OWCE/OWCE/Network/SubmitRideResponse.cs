using System;
using System.Text.Json.Serialization;

namespace OWCE.Network
{
    public class SubmitRideResponse
    {
        [JsonPropertyName("id")]
        public string ID { get; set; } = String.Empty;

        [JsonPropertyName("upload_url")]
        public string UploadUrl { get; set; } = String.Empty;
    }
}

