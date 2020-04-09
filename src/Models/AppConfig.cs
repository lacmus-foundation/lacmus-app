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
        public Language Language { get; set; }
        public string BorderColor { get; set; }
        public ThemeManager.Theme Theme { get; set; }
        public MLModelConfig MlModelConfig { get; set; }
        
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