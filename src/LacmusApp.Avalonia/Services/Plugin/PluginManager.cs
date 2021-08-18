using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using LacmusApp.Avalonia.Models.Plugins;
using LacmusPlugin;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Serilog;
using Version = LacmusPlugin.Version;

namespace LacmusApp.Avalonia.Services.Plugin
{
    public class PluginManager
    {
        private string _baseDirectory;
        public PluginManager(string baseDirectory)
        { 
            _baseDirectory = baseDirectory;
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
        
        public async Task<List<IObjectDetectionPlugin>> GetPluginsFromRepositoryAsync(PluginRepository repository)
        {
            var result = new List<IObjectDetectionPlugin>();
            var maxPageUrl = Url.Combine(repository.Url, "/plugin-repository/api/v1/pagesCount");
            var pageCount = JsonConvert.DeserializeObject<PluginPageCount>(await GetAsync(maxPageUrl));
            for (int i = 0; i < pageCount.Count; i++)
            {
                var pluginInfoUrl = Url.Combine(repository.Url, $"/plugin-repository/api/v1/plugins?page={i}");
                var plugins =
                    JsonConvert.DeserializeObject<IEnumerable<PluginInfo>>(await GetAsync(pluginInfoUrl));
                result.AddRange(plugins);
            }

            return result;
        }

        public IObjectDetectionPlugin GetPlugin(string tag, Version version)
        {
            var dir = Path.Join(_baseDirectory, tag, version.ToString());
            if (!Directory.Exists(dir))
                throw new InvalidOperationException($"no such plugin {tag}-{version.ToString()}");
            
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
        
        public async Task<IObjectDetectionPlugin> GetPluginsAsync(string tag, Version version)
        {
            return await Task.Run(() => GetPlugin(tag, version));
        }

        public async Task InstallPlugin(IObjectDetectionPlugin plugin, PluginRepository repository)
        {
            var url = Url.Combine(repository.Url, "/plugin-repository/api/v1/plugin?",
                $"tag={plugin.Tag}", $"api={plugin.Version.Api}", 
                $"major={plugin.Version.Major}", $"minor={plugin.Version.Minor}");
            Log.Information($"Downloading {plugin.Tag}-{plugin.Version.ToString()} plugin");
            await using (var stream = await GetFileAsync(url))
            {
                Log.Information($"Extracting {plugin.Tag}-{plugin.Version.ToString()} plugin");
                using (var archive = new ZipArchive(stream))
                {
                    var baseDir = Path.Combine(_baseDirectory,
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
            GC.Collect();
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
        private async Task<string> GetAsync(string uri)
        {
            using (var httpClient = new HttpClient())
                return await httpClient.GetStringAsync(uri);
        }
        
        private async Task<Stream> GetFileAsync(string uri)
        {
            using (var httpClient = new HttpClient())
                return await httpClient.GetStreamAsync(uri);
        }

    }
}