using System.Collections.Generic;
using LacmusPlugin;
using LacmusPlugin.Enums;

namespace LacmusApp.Services.Plugin
{
    public class PluginInfo : IObjectDetectionPlugin
    {
        public string Tag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Company { get; set; }
        public string Url { get; set; }
        public Version Version { get; set; }
        public InferenceType InferenceType { get; set; }
        public HashSet<OperatingSystem> OperatingSystems { get; set; }
        
        public IObjectDetectionModel LoadModel(float threshold)
        {
            throw new System.NotImplementedException();
        }
    }
}