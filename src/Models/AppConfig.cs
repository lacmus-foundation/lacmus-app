using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LacmusApp.Managers;
using LacmusApp.Models.Plugins;
using LacmusApp.Services.Plugin;
using LacmusPlugin.Enums;
using Newtonsoft.Json;
using Octokit;
using Serilog;
using Language = LacmusApp.Services.Language;
using OperatingSystem = LacmusPlugin.OperatingSystem;
using Version = LacmusPlugin.Version;

namespace LacmusApp.Models
{
    [JsonObject]
    public class AppConfig
    {
        private string _borderColor = "#FFFF0000";
        private PluginRepository _repository = new("lacmus", "http://176.99.131.182:80");

        private string _configDir =
            Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lacmus");
        private string _pluginDir = 
            Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lacmus", "plugins");
        
        private PluginInfo _pluginInfo = new()
        {
            Author = "gosha20777",
            Company = "Lacmus Foundation",
            Description = "Resnet50+deepFPN neural network",
            Name = "Lacmus Retinanet",
            Tag = "LacmusRetinanetPlugin.Cpu",
            Url = "https://github.com/lacmus-foundation/lacmus",
            Version = new(api: 2, major: 1, minor: 0),
            InferenceType = InferenceType.Cpu,
            OperatingSystems = new HashSet<OperatingSystem>()
            {
                OperatingSystem.LinuxAmd64,
                OperatingSystem.WindowsAmd64,
                OperatingSystem.OsxAmd64
            }
        };
        
        public Language Language { get; set; } = Language.English;

        public string BorderColor
        {
            get => _borderColor;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !value.Contains('#') || value.Length != 9)
                {
                    throw new Exception($"invalid BorderColor: {_borderColor}");
                }

                _borderColor = value;
            }
        } 
        public ThemeManager.Theme Theme { get; set; }

        public PluginRepository Repository
        {
            get => _repository;
            set => _repository = value;
        }
        
        public string PluginDir
        {
            get => _pluginDir;
            set => _pluginDir = value;
        }

        public PluginInfo PluginInfo
        {
            get => _pluginInfo;
            set => _pluginInfo = value;
        }

        public async Task Save(string path)
        {
            try
            {
                var str = JsonConvert.SerializeObject(this);
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                await File.WriteAllTextAsync(path, str);
                Log.Debug($"Config saved to {path}.");
                Log.Debug(_pluginInfo.Tag);
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to save config to file {path}.", e);
            }
        }
        
        public async Task Save()
        {
            var configPath = Path.Join(_configDir,"appConfig-v2.json");
            await Save(configPath);
        }
        
        public static async Task<AppConfig> Create(string path)
        {
            try
            {
                if (!File.Exists(path))
                    throw new Exception($"unable to load config file. Bo such file {path}.");
                var str = await File.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject<AppConfig>(str);
            }
            catch (Exception e)
            {
                throw new Exception($"unable to load config from {path}.", e);
            }
        }

        public static AppConfig DeepCopy(AppConfig config)
        {
            //deep copy ml config
            var newConfig = new AppConfig();
            newConfig.Language = config.Language;
            newConfig.Repository = config.Repository;
            newConfig.BorderColor = config.BorderColor;
            newConfig.Theme = config.Theme;
            newConfig.PluginDir = config.PluginDir;
            newConfig.PluginInfo = config.PluginInfo;
            return newConfig;
        }
    }
}