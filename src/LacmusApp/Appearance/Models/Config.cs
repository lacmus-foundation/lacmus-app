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
    public class Config : IConfig
    {
        [JsonProperty("repository")]
        public string Repository { get; set; } = "http://api.lacmus.ml";
        [JsonProperty("language")]
        public Language Language { get; set; } = Language.English;
        [JsonProperty("theme")]
        public Theme Theme { get; set; } = Theme.Light;
        [JsonProperty("boundingBoxColour")]
        public BoundingBoxColour BoundingBoxColour { get; set; } = BoundingBoxColour.Red;
        [JsonProperty("predictionThreshold")]
        public float PredictionThreshold { get; set; } = 0.15f;
        [JsonProperty("plugin")]
        public PluginInfo Plugin { get; set; } = new()
        {
            Author = "gosha20777",
            Company = "Lacmus Foundation",
            Description = "Resnet50+deepFPN neural network",
            Name = "Lacmus Retinanet",
            Tag = "LacmusRetinanetPlugin.Cpu",
            Url = "https://github.com/lacmus-foundation/lacmus",
            Version = new Version(api: 2, major: 5, minor: 0),
            InferenceType = InferenceType.Cpu,
            OperatingSystems = new HashSet<OperatingSystem>()
            {
                OperatingSystem.LinuxAmd64,
                OperatingSystem.WindowsAmd64,
                OperatingSystem.OsxAmd64
            }
        };
    }
}