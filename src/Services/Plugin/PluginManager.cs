using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LacmusApp.Models.Plugins;
using LacmusPlugin;
using Serilog;
using Version = LacmusPlugin.Version;

namespace LacmusApp.Services.Plugin
{
    public class PluginManager
    {
        private string _baseDirectory;
        private PluginRepository[] _repositories;
        public PluginManager(string baseDirectory, PluginRepository[] repositories)
        { 
            _baseDirectory = baseDirectory;
            _repositories = repositories;
        }
        
        public List<IObjectDetectionPlugin> GetInstalledPlugins()
        {
            var pluginPaths = Directory.GetFiles(_baseDirectory, "*.dll", new EnumerationOptions() {RecurseSubdirectories = true});
            return pluginPaths.SelectMany(pluginPath =>
            {
                try
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreatePlugins(pluginAssembly);
                }
                catch
                {
                    Console.WriteLine("cannot load assembly from {0}", pluginPath);
                    return new List<IObjectDetectionPlugin>();
                }
            }).ToList();
        }
        public async Task<List<IObjectDetectionPlugin>> GetInstalledPluginsAsync()
        {
            return await Task.Run(GetInstalledPlugins);
        }
        
        public IEnumerable<IObjectDetectionPlugin> GetPluginsFromRepository(PluginRepository repository)
        {
            throw new System.NotImplementedException();
        }

        public IObjectDetectionPlugin GetPlugin(string tag, Version version)
        {
            var dir = Path.Join(_baseDirectory, tag, version.ToString());
            if (!Directory.Exists(dir))
                throw new InvalidOperationException("no such plugin");
            
            var pluginPaths = Directory.GetFiles(dir, "*.dll", new EnumerationOptions() {RecurseSubdirectories = true});
            var plugins = pluginPaths.SelectMany(pluginPath =>
            {
                try
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreatePlugins(pluginAssembly);
                }
                catch
                {
                    Log.Warning("cannot load assembly from {0}", pluginPath);
                    return new List<IObjectDetectionPlugin>();
                }
            }).ToList();
            foreach (var plugin in plugins)
            {
                if (plugin.Tag == tag && plugin.Version.ToString() == version.ToString())
                    Log.Information($"Load plugin {tag}-{version.ToString()}");
                    return plugin;
            }

            throw new InvalidOperationException($"No such plugin {tag}-{version.ToString()}");
        }

        public void InstallPlugin(IObjectDetectionPlugin plugin)
        {
            throw new System.NotImplementedException();
        }

        public void RemovePlugin(IObjectDetectionPlugin plugin)
        {
            throw new System.NotImplementedException();
        }
        
        private static Assembly LoadPlugin(string path)
        {
            PluginLoadContext loadContext = new PluginLoadContext(path);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
        }
        
        private static IEnumerable<IObjectDetectionPlugin> CreatePlugins(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IObjectDetectionPlugin).IsAssignableFrom((Type?) (Type?) type))
                {
                    if (Activator.CreateInstance(type) is IObjectDetectionPlugin result)
                    {
                        yield return result;
                    }
                }
            }
        }
    }
}