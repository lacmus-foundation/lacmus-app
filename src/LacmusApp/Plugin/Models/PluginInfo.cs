using System.Collections.Generic;
using LacmusPlugin;
using LacmusPlugin.Enums;
using Newtonsoft.Json;

namespace LacmusApp.Plugin.Models
{
    [JsonObject]
    public class PluginInfo : IObjectDetectionPlugin
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("dependences")]
        public IEnumerable<string> Dependences { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("version")]
        public Version Version { get; set; }
        public string StringVersion => Version.ToString();
        [JsonProperty("inferenceType")]
        public InferenceType InferenceType { get; set; }
        [JsonProperty("operatingSystems")]
        public HashSet<OperatingSystem> OperatingSystems { get; set; }
        
        public IObjectDetectionModel LoadModel(float threshold)
        {
            throw new System.NotImplementedException();
        }
    }
}