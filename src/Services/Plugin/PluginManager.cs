using System.Collections.Generic;
using LacmusApp.Models;
using LacmusApp.Models.Plugins;
using LacmusPlugin;

namespace LacmusApp.Services.Plugin
{
    public class PluginManager : IPluginManager
    {
        private readonly AppConfig _config;
        public PluginManager(AppConfig config)
        {
            _config = config;
        }

        public IEnumerable<IObjectDetectionPlugin> GetPluginsFromRepository(PluginRepository repository)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IObjectDetectionPlugin> GetInstalledPlugins()
        {
            throw new System.NotImplementedException();
        }

        public IObjectDetectionPlugin GetCurrentPlugin()
        {
            throw new System.NotImplementedException();
        }

        public void InstallPlugin(IObjectDetectionPlugin plugin)
        {
            throw new System.NotImplementedException();
        }

        public void RemovePlugin(IObjectDetectionPlugin plugin)
        {
            throw new System.NotImplementedException();
        }
    }
}