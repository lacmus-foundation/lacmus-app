using LacmusApp.Plugin.Models;
using LacmusPlugin;

namespace LacmusApp.Plugin.Extensions
{
    public static class PluginInfoExtension
    {
        public static PluginInfo GetPluginInfo(this IObjectDetectionPlugin plugin)
        {
            return new ()
            {
                Tag = plugin.Tag,
                Name = plugin.Name,
                Description = plugin.Description,
                Author = plugin.Author,
                Company = plugin.Company,
                Url = plugin.Url,
                Version = plugin.Version,
                InferenceType = plugin.InferenceType,
                OperatingSystems = plugin.OperatingSystems
            };
        }
    }
}