using System.Linq;
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
                OperatingSystems = plugin.OperatingSystems,
                Dependences = plugin.Dependences
            };
        }

        public static string GetDependenciesAsString(this IObjectDetectionPlugin plugin)
        {
            if (plugin.Dependences == null)
                return "";
            return plugin.Dependences.Aggregate(
                string.Empty, 
                (current, dependency) => current + $"{dependency};");
        }

        public static string GetOperatingSystemsAsString(this IObjectDetectionPlugin plugin)
        {
            if (plugin.OperatingSystems == null)
                return "";
            var result = string.Empty;
            foreach (var os in plugin.OperatingSystems)
            {
                result += os switch
                {
                    OperatingSystem.AndroidArm => "Android",
                    OperatingSystem.IosArm => "IOS",
                    OperatingSystem.LinuxAmd64 => "Linux",
                    OperatingSystem.LinuxArm => "Linux (ARM)",
                    OperatingSystem.OsxAmd64 => "OSX (amd64)",
                    OperatingSystem.OsxArm => "OSX (Apple Silicon)",
                    OperatingSystem.WindowsAmd64 => "Windows",
                    OperatingSystem.WindowsArm => "Windows (ARM)",
                    _ => os.ToString()
                };
                result += ";";
            }

            return result;
        }
    }
}