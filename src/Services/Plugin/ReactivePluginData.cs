using ReactiveUI.Fody.Helpers;

namespace LacmusApp.Services.Plugin
{
    public class ReactivePluginData
    {
        [Reactive] public string Name { get; set; }
        //[Reactive] public MLModelType Type { get; set; }
        [Reactive] public uint Version { get; set; }
        [Reactive] public uint ApiVersion { get; set; }

        public ReactivePluginData(string name, uint version, uint apiVersion)
        {
            Name = name;
            Version = version;
            ApiVersion = apiVersion;
        }
    }
}