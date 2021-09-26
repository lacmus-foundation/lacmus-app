using LacmusApp.Appearance.Enums;
using LacmusApp.Plugin.Models;

namespace LacmusApp.Appearance.Interfaces
{
    public interface IConfig
    {
        string Repository { get; set; }
        float PredictionThreshold { get; set; }
        PluginInfo Plugin { get; set; }
        Language Language { get; set; }
        Theme Theme { get; set; }
        BoundingBoxColour BoundingBoxColour { get; set; }
    }
}