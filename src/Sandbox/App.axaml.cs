using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LacmusApp.Appearance.Enums;
using LacmusApp.Appearance.Models;
using LacmusApp.Plugin.Models;
using LacmusApp.Plugin.Services;
using Sandbox.Views;
using LacmusApp.Plugin.ViewModels;
using LacmusApp.Screens.ViewModels;
using LacmusPlugin;
using LacmusPlugin.Enums;
using Sandbox.Services;

namespace Sandbox
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var pluginManager = new PluginManager(
                    "plugins", "http://api.lacmus.ml");
                var configManager = new ConfigManager("config.json");
                //TODO: load config
                var config = new Config()
                {
                    Language = Language.English,
                    Plugin = new PluginInfo()
                    {
                        Author = "gosha207771",
                        Company = "Lacmus Foundation",
                        Description = "Resnet50+deepFPN neural network",
                        Name = "Lacmus Retinanet",
                        Tag = "LacmusRetinanetPlugin.Cpu",
                        Url = "https://github.com/lacmus-foundation/lacmus",
                        Version = new(api: 2, major: 5, minor: 0),
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
                    PredictionThreshold = 0.80f,
                    BoundingBoxColour = BoundingBoxColour.Blue
                };

                var fileManager = new AvaloniaPluginFileManager(new Window());

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new SettingsViewModel(
                        config,
                        configManager,
                        pluginManager,
                        fileManager)
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}