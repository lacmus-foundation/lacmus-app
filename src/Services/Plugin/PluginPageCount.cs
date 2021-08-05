using Newtonsoft.Json;

namespace LacmusApp.Services.Plugin
{
    [JsonObject]
    public class PluginPageCount
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}