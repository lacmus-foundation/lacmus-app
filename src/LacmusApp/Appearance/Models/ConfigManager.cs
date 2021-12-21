using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LacmusApp.Appearance.Enums;
using LacmusApp.Appearance.Interfaces;
using LacmusApp.Plugin.Models;
using LacmusPlugin.Enums;
using Newtonsoft.Json;
using Serilog;
using OperatingSystem = LacmusPlugin.OperatingSystem;

namespace LacmusApp.Appearance.Models
{
    public class ConfigManager : IConfigManager
    {
        private readonly string _configPath;
        
        public ConfigManager(string configPath)
        {
            _configPath = configPath;
        }
        
        public async Task<Config> ReadConfig()
        {
            Log.Information($"Load config from {_configPath}");
            try
            {
                var configStr = await File.ReadAllTextAsync(_configPath);
                return JsonConvert.DeserializeObject<Config>(configStr);
            }
            catch (Exception e)
            {
                Log.Warning($"Con not parse config from {_configPath}.", e);
                var config = new Config()
                {
                    Language = Language.English,
                    Plugin = new PluginInfo()
                    {
                        Author = "gosha207771",
                        Company = "Lacmus Foundation",
                        Description = "Resnet50+deepFPN neural network",
                        Name = "Lacmus Retinanet",
                        Tag = "LacmusRetinanetPlugin.Cpu",
                        Url = "https://github.com/lacmus-foundation/lacmus",
                        Version = new(api: 2, major: 5, minor: 0),
                        InferenceType = InferenceType.Cpu,
                        OperatingSystems = new HashSet<OperatingSystem>()
                        {
                            OperatingSystem.LinuxAmd64,
                            OperatingSystem.WindowsAmd64,
                            OperatingSystem.OsxAmd64
                        }
                    },
                    Repository = "http://api.lacmus.ml",
                    Theme = Theme.Light,
                    PredictionThreshold = 0.80f,
                    BoundingBoxColour = BoundingBoxColour.Blue
                };
                await SaveConfig(config);
                return config;
            }
        }

        public async Task SaveConfig(Config config)
        {
            try
            {
                var configStr = JsonConvert.SerializeObject(config);
                await File.WriteAllTextAsync(_configPath, configStr);
            }
            catch (Exception e)
            {
                Log.Error($"Can not save config to {_configPath}.", e);
                throw new Exception($"Can not save config to {_configPath}.");
            }
        }
    }
}