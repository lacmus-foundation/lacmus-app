using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LacmusApp.Plugin.Interfaces;
using LacmusPlugin;

namespace LacmusApp.Plugin.Services
{
    //TODO: Add logging
    public class PluginManager : IPluginManager
    {
        public string BaseDirectory { get; }
        public string BaseApiUrl { get; }

        public PluginManager(string baseDirectory, string baseApiUrl)
        {
            BaseDirectory = baseDirectory;
            BaseApiUrl = baseApiUrl;
        }
        
        public async Task<IReadOnlyCollection<IObjectDetectionPlugin>> GetPluginsFromRepository()
        {
            var resultList = new List<IObjectDetectionPlugin>();
            using (var client = new WebClient(BaseApiUrl))
            {
                var count = await client.GetMaxPage();
                for (var i = 0; i < count; i++)
                    resultList.AddRange(await client.GetPluginInfoFromPage(i));
            }
            return resultList;
        }

        public async Task<IReadOnlyCollection<IObjectDetectionPlugin>> GetInstalledPlugins()
        {
            var pluginPaths = Directory.GetFiles(BaseDirectory, "*.dll", new EnumerationOptions() {RecurseSubdirectories = true});
            return await Task.Run(() =>
            {
                return pluginPaths.SelectMany(pluginPath =>
                {
                    try
                    {
                        Assembly pluginAssembly = LoadAssembly(pluginPath);
                        return CreatePluginsFromAssembly(pluginAssembly);
                    }
                    catch
                    {
                        Console.WriteLine("cannot load assembly from {0}", pluginPath);
                        return new List<IObjectDetectionPlugin>();
                    }
                }).ToList();
            });
        }

        public async Task InstallPlugin(IObjectDetectionPlugin plugin)
        {
            using (var client = new WebClient(BaseApiUrl))
            {
                using (var stream = await client.GetZipFile(
                    plugin.Tag, plugin.Version.Api, 
                    plugin.Version.Major, plugin.Version.Minor))
                {
                    using (var archive = new ZipArchive(stream))
                    {
                        var baseDir = Path.Combine(BaseDirectory,
                            plugin.Tag, plugin.Version.ToString());
                        Directory.CreateDirectory(baseDir);
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            var fullPath = Path.Combine(baseDir, entry.FullName);
                            if (String.IsNullOrEmpty(entry.Name))
                                Directory.CreateDirectory(fullPath);
                            else
                                entry.ExtractToFile(fullPath);
                        }
                    }
                }
            }
            GC.Collect();
        }

        public Task UninstallPlugin(IObjectDetectionPlugin plugin)
        {
            throw new System.NotImplementedException();
        }

        public Task<IObjectDetectionPlugin> LoadPlugin(IObjectDetectionPlugin plugin)
        {
            throw new System.NotImplementedException();
        }

        private Assembly LoadAssembly(string path)
        {
            PluginLoadContext loadContext = new PluginLoadContext(path);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
        }
        
        private static IEnumerable<IObjectDetectionPlugin> CreatePluginsFromAssembly(Assembly assembly)
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