using LacmusApp.Plugin.Interfaces;
using Newtonsoft.Json;

namespace LacmusApp.Plugin.Models
{
    [JsonObject]
    public class MaxPageCount : IPageCount
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}