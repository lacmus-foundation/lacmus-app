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
        Task InstallPlugin(IObjectDetectionPlugin plugin);
        Task UninstallPlugin(IObjectDetectionPlugin plugin);
        Task<IObjectDetectionPlugin> LoadPlugin(IObjectDetectionPlugin plugin);
        String BaseDirectory { get; }
        String BaseApiUrl { get; }
    }
}