using Newtonsoft.Json;

namespace LacmusApp.Avalonia.Services.Plugin
{
    [JsonObject]
    public class PluginPageCount
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}