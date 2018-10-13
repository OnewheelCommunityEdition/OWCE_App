using System;
using RestSharp.Deserializers;

namespace OWCE
{
    public class BaseResponse
    {
        [DeserializeAs(Name = "success")]
        public bool Success { get; set; }

        [DeserializeAs(Name = "message")]
        public string Message { get; set; }
    }

    public class BoardGetResponse : BaseResponse
    {
        [DeserializeAs(Name = "name")]
        public string Name { get; set; }
    }
}
