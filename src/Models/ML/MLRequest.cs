using System.Collections.Generic;
using Newtonsoft.Json;

namespace LacmusApp.Models.ML
{
    [JsonObject]
    public class MLRequest
    {
        [JsonProperty("data")]
        public byte[] Data { get; set; }
    }
}