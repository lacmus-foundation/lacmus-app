using System.Collections.Generic;
using LacmusApp.Appearance.Enums;
using LacmusApp.Appearance.Interfaces;
using LacmusApp.Plugin.Models;
using LacmusPlugin;
using LacmusPlugin.Enums;
using Newtonsoft.Json;

namespace LacmusApp.Appearance.Models
{
    [JsonObject]
    public struct Config
    {
        [JsonProperty("repository")]
        public string Repository { get; set; }
        [JsonProperty("language")]
        public Language Language { get; set; }
        [JsonProperty("theme")]
        public Theme Theme { get; set; }
        [JsonProperty("boundingBoxColour")]
        public BoundingBoxColour BoundingBoxColour { get; set; }
        [JsonProperty("predictionThreshold")]
        public float PredictionThreshold { get; set; }
        [JsonProperty("plugin")]
        public PluginInfo Plugin { get; set; }
    }
}