using System;
using System.IO;
using System.Threading.Tasks;
using LacmusApp.Appearance.Interfaces;
using Newtonsoft.Json;
using Serilog;

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
            try
            {
                var configStr = await File.ReadAllTextAsync(_configPath);
                return JsonConvert.DeserializeObject<Config>(configStr);
            }
            catch (Exception e)
            {
                Log.Warning($"Con not parse config from {_configPath}.", e);
                var config = new Config();
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