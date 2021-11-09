using System;
using System.Text.Json.Serialization;

namespace OWCE.Network
{
    public class HandshakeStatusResponse
    {
        [JsonPropertyName("online")]
        public bool Online { get; set; } = false;

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
