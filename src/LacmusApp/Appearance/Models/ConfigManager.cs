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
                        Author = "Ivan",
                        Company = "Lacmus Foundation",
                        Description = "YOLO v5 neural network",
                        Name = "Lacmus YOLO v5",
                        Tag = "LacmusYolo5Plugin.Cpu",
                        Url = "https://github.com/lacmus-foundation/lacmus-research",
                        Version = new(api: 2, major: 1, minor: 1),
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
                    PredictionThreshold = 0.15f,
                    BoundingBoxColour = BoundingBoxColour.Red
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
                Log.Information($"Save config to {_configPath}");
            }
            catch (Exception e)
            {
                Log.Error($"Can not save config to {_configPath}.", e);
                throw new Exception($"Can not save config to {_configPath}.");
            }
        }
    }
}