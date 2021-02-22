using System.Collections.Generic;
using LacmusApp.Models.Plugins;
using LacmusPlugin;

namespace LacmusApp.Services.Plugin
{
    public interface IPluginManager
    {
        public IEnumerable<IObjectDetectionPlugin> GetPluginsFromRepository(PluginRepository repository);
        public IEnumerable<IObjectDetectionPlugin> GetInstalledPlugins();
        public IObjectDetectionPlugin GetCurrentPlugin();
        public void InstallPlugin(IObjectDetectionPlugin plugin);
        public void RemovePlugin(IObjectDetectionPlugin plugin);
    }
}