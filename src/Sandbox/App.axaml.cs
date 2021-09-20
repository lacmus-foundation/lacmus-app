using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LacmusApp.Plugin.Models;
using LacmusApp.Plugin.Services;
using Sandbox.Views;
using LacmusApp.Plugin.ViewModels;
using LacmusApp.Screens.ViewModels;
using LacmusPlugin;
using LacmusPlugin.Enums;

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
                var pluginInfo = new PluginInfo()
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
                };
                var pluginViewModel = new PluginViewModel(
                    pluginInfo,
                    pluginManager);
                var localPluginRepositoryViewModel = new LocalPluginRepositoryViewModel();
                var remotePluginRepositoryViewModel = new RemotePluginRepositoryViewModel(
                    pluginManager);
                
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new SettingsViewModel(
                        localPluginRepositoryViewModel,
                        remotePluginRepositoryViewModel,
                        pluginViewModel)
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}