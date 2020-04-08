using Avalonia.Media;
using LacmusApp.Managers;
using LacmusApp.Models.ML;
using LacmusApp.Services.Files;

namespace LacmusApp.Models
{
    public static class AppConfig
    {
        public static Language Language { get; set; }
        public static string BorderColor { get; set; }
        public static ThemeManager.Theme Theme { get; set; }
        public static MLModelConfig MlModelConfig { get; set; }
    }
}