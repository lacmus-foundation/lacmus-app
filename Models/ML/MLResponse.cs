using System.Collections.Generic;
using Newtonsoft.Json;

namespace RescuerLaApp.Models.ML
{
    public class MLResponse
    {
        [JsonProperty("objects")]
        public List<PredictObject> Objects { get; set; } = new List<PredictObject>();
        
        [JsonObject]
        public class PredictObject
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("score")]
            public string Score { get; set; }
            [JsonProperty("xmin")]
            public int Xmin { get; set; }
            [JsonProperty("xmax")]
            public int Xmax { get; set; }
            [JsonProperty("ymin")]
            public int Ymin { get; set; }
            [JsonProperty("ymax")]
            public int Ymax { get; set; }
        
        }
    }
}