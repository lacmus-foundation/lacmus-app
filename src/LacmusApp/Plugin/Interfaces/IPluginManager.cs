using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LacmusPlugin;

namespace LacmusApp.Plugin.Interfaces
{
    public interface IPluginManager
    {
        Task<IReadOnlyCollection<IObjectDetectionPlugin>> GetPluginsFromRepository();
        Task<IReadOnlyCollection<IObjectDetectionPlugin>> GetInstalledPlugins();
        Task ImportPlugin(string path);
        Task InstallPlugin(IObjectDetectionPlugin plugin);
        Task UninstallPlugin(IObjectDetectionPlugin plugin);
        Task<IObjectDetectionPlugin> LoadPlugin(string tag, LacmusPlugin.Version version);
        String BaseDirectory { get; }
        String BaseApiUrl { get; }
    }
}