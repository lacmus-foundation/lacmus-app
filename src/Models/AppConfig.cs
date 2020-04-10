using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media;
using LacmusApp.Managers;
using LacmusApp.Models.ML;
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
    }
}