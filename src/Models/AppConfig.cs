using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media;
using LacmusApp.Managers;
using LacmusApp.Models.Docker;
using LacmusApp.Models.ML;
using LacmusApp.Services;
using LacmusApp.Services.Files;
using Newtonsoft.Json;
using Serilog;

namespace LacmusApp.Models
{
    [JsonObject]
    public class AppConfig
    {
        private string _borderColor = "#FFFF0000";
        private MLModelConfig _mlModelConfig = new MLModelConfig();
        private string[] _repositories = 
        {
            "gosha20777/lacmus",
            "gosha20777/lacmus-kseniia"
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

        public string[]  Repositories
        {
            get => _repositories;
            set
            {
                if(value == null || value.Length < 1)
                {
                    throw new Exception($"invalid Repositories: {_repositories}");
                }
                _repositories = value;
            }
        } 

        public MLModelConfig MlModelConfig
        {
            get => _mlModelConfig;
            set
            {
                _mlModelConfig = value ?? throw new Exception($"MlModelConfig is null");
            }
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
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to save config to file {path}.", e);
            }
        }
        
        public async Task Save()
        {
            var confDir = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "conf");
            var configPath = Path.Join(confDir,"appConfig.json");
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
            newConfig.Repositories = new List<string>(config.Repositories).ToArray();
            newConfig.BorderColor = config.BorderColor;
            newConfig.MlModelConfig = new MLModelConfig();
            newConfig.MlModelConfig.Accaunt = new DockerAccaunt();
            newConfig.MlModelConfig.Accaunt.Email = config.MlModelConfig.Accaunt.Email;
            newConfig.MlModelConfig.Accaunt.Password = config.MlModelConfig.Accaunt.Password;
            newConfig.MlModelConfig.Accaunt.Username = config.MlModelConfig.Accaunt.Username;
            newConfig.MlModelConfig.Image = new DockerImage();
            newConfig.MlModelConfig.Image.Name = config.MlModelConfig.Image.Name;
            newConfig.MlModelConfig.Image.Tag = config.MlModelConfig.Image.Tag;
            newConfig.MlModelConfig.Type = config.MlModelConfig.Type;
            newConfig.MlModelConfig.Url = config.MlModelConfig.Url;
            newConfig.MlModelConfig.ApiVersion = config.MlModelConfig.ApiVersion;
            newConfig.MlModelConfig.ModelVersion = config.MlModelConfig.ModelVersion;
            return newConfig;
        }
    }
}