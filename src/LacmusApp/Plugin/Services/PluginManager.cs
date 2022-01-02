using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LacmusApp.Plugin.Interfaces;
using LacmusPlugin;
using Serilog;

namespace LacmusApp.Plugin.Services
{
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
            try
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
            catch (Exception e)
            {
                Log.Error(e, $"Can not get plugins from remote repository {BaseApiUrl}");
                throw new Exception("Can not get plugins from remote repository.");
            }
        }

        public async Task<IReadOnlyCollection<IObjectDetectionPlugin>> GetInstalledPlugins()
        {
            try
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
            catch (Exception e)
            {
                Log.Error(e, $"Can not get plugins from local repository {BaseDirectory}");
                throw new Exception("Can not get plugins from local repository.");
            }
        }

        public async Task ImportPlugin(string path)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (var stream = File.OpenRead(path))
                    {
                        Log.Information($"Import plugin from {path}...");
                        using (var archive = new ZipArchive(stream))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                var fullPath = Path.Combine(BaseDirectory, entry.FullName);
                                if (String.IsNullOrEmpty(entry.Name))
                                    Directory.CreateDirectory(fullPath);
                                else
                                    entry.ExtractToFile(fullPath);
                            }
                        }
                    }
                    GC.Collect();
                    Log.Information($"Plugin was imported.");
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Can not import plugin from {path}");
                    throw new Exception("Can not import plugin.");
                }
            });
        }

        public async Task InstallPlugin(IObjectDetectionPlugin plugin)
        {
            await Task.Run(async () =>
            {
                try
                {
                    Log.Information($"Downloading plugin {plugin.Tag}-{plugin.Version.ToString()}...");
                    using (var client = new WebClient(BaseApiUrl))
                    {
                        using (var stream = await client.GetZipFile(
                            plugin.Tag, plugin.Version.Api,
                            plugin.Version.Major, plugin.Version.Minor))
                        {
                            using (var archive = new ZipArchive(stream))
                            {
                                Log.Information($"Installing plugin {plugin.Tag}-{plugin.Version.ToString()}...");
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
                    Log.Information($"The plugin {plugin.Tag}-{plugin.Version.ToString()} was installed.");
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Can not install plugin {plugin.Tag}-{plugin.Version.ToString()}");
                    throw new Exception("Can not install plugin.");
                }
            });
        }

        public async Task UninstallPlugin(IObjectDetectionPlugin plugin)
        {
            try
            {
                Log.Information($"Uninstalling plugin {plugin.Tag}-{plugin.Version.ToString()}...");
                await Task.Run(() => Directory.Delete
                (
                    Path.Combine(BaseDirectory, plugin.Tag, plugin.Version.ToString()), true
                ));
                Log.Information($"The plugin {plugin.Tag}-{plugin.Version.ToString()} was removed.");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Can not uninstall plugin {plugin.Tag}-{plugin.Version.ToString()}");
                throw new Exception("Can not uninstall plugin.");
            }
        }

        public async Task<IObjectDetectionPlugin> LoadPlugin(string tag, LacmusPlugin.Version version)
        {
            var path = Path.Join(BaseDirectory, tag, version.ToString());
            if (!Directory.Exists(path))
            {
                Log.Error($"No such plugin {tag}-{version.ToString()}");
                throw new InvalidOperationException($"No such plugin {tag}-{version.ToString()}");
            }
            
            return await Task.Run(() =>
            {
                var pluginPaths = Directory.GetFiles(path, "*.dll", new EnumerationOptions() {RecurseSubdirectories = true});
                var plugins = pluginPaths.SelectMany(pluginPath =>
                {
                    try
                    {
                        Assembly pluginAssembly = LoadAssembly(pluginPath);
                        return CreatePluginsFromAssembly(pluginAssembly);
                    }
                    catch
                    {
                        return new List<IObjectDetectionPlugin>();
                    }
                }).ToList();
                foreach (var p in plugins.Where(
                    p => tag == p.Tag && 
                         version.ToString() == p.Version.ToString()))
                {
                    return p;
                }
                Log.Error($"Can not load plugin {tag}-{version.ToString()}");
                throw new InvalidOperationException($"Can not load plugin {tag}-{version.ToString()}");
            });
        }

        private static Assembly LoadAssembly(string path)
        {
            var loadContext = new PluginLoadContext(path);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
        }
        
        private static IEnumerable<IObjectDetectionPlugin> CreatePluginsFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
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