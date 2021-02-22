namespace LacmusApp.Models.Plugins
{
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