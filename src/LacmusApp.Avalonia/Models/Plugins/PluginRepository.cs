using Newtonsoft.Json;

namespace LacmusApp.Avalonia.Models.Plugins
{
    [JsonObject]
    public readonly struct PluginRepository
    {
        public PluginRepository(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }
        public string Url { get; }
        public string Name { get; }
    }
}