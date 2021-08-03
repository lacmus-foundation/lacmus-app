using System.Collections.Generic;
using LacmusPlugin;
using LacmusPlugin.Enums;
using Newtonsoft.Json;

namespace LacmusApp.Services.Plugin
{
    [JsonObject]
    public class PluginInfo : IObjectDetectionPlugin
    {
        public PluginInfo() { }
        public PluginInfo(IObjectDetectionPlugin plugin)
        {
            Tag = plugin.Tag;
            Name = plugin.Name;
            Description = plugin.Description;
            Author = plugin.Author;
            Company = plugin.Company;
            Url = plugin.Url;
            Version = plugin.Version;
            InferenceType = plugin.InferenceType;
            OperatingSystems = plugin.OperatingSystems;
        }
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