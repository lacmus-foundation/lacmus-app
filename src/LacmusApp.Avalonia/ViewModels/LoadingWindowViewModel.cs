using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using LacmusApp.Appearance.Models;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Services;
using LacmusApp.Avalonia.Views;
using LacmusApp.Plugin.Services;
using LacmusApp.Screens.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace LacmusApp.Avalonia.ViewModels
{
    public class LoadingWindowViewModel : ReactiveObject
    {
        private readonly Window _window;
        
        public LoadingWindowViewModel(Window window)
        {
            _window = window;
            InitCommand = ReactiveCommand.Create(Init);
            
            InitCommand.Execute().Subscribe();
        }
        
        [Reactive] public string TextVersion { get; set; } = GetVersion() + ".";
        public ReactiveCommand<Unit, Unit> InitCommand { get; set; }

        private async void Init()
        {
            var window = new MainWindow();
            await Task.Delay(1000);
            var confDir = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "lacmus");
            var configPath = Path.Join(confDir, "appConfig-v3.json");
            var configManager = new ConfigManager(configPath);
            var config = await configManager.ReadConfig();
            var fileManager = new AvaloniaPluginFileManager(window);
            var pluginManager = new PluginManager(
                Path.Join(confDir, "plugins"), "http://api.lacmus.ml");
            var themeManager = new ThemeManager(window);
            themeManager.UseTheme(config.Theme);
            var settingsViewModel = new SettingsViewModel(
                config,
                configManager,
                pluginManager,
                fileManager);
            
            window.DataContext = new MainWindowViewModel(
                window,
                settingsViewModel,
                themeManager);
            
            window.Show();
            _window.Close();
        }
        
        private async Task<AppConfig> LoadConfig()
        {
            var confDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lacmus");
            var configPath = Path.Join(confDir,"appConfig-v2.json");
            if (File.Exists(configPath))
                try
                {
                    return await AppConfig.Create(configPath);
                }
                catch (Exception e)
                {
                    Log.Error($"Unable to parse config from {configPath}.", e);

                    var config = new AppConfig();
                    await config.Save(configPath);
                    Log.Information("Create default config.");
                    return config;
                }
            else
            {
                if (!Directory.Exists(confDir))
                    Directory.CreateDirectory(confDir);
                
                var config = new AppConfig();
                await config.Save(configPath);
                Log.Information("Create default config.");
                return config;
            }
        }
        private static string GetVersion()
        {
            var revision = "";
            if (typeof(Program).Assembly.GetName().Version.Revision != 0)
                revision = $"preview-{typeof(Program).Assembly.GetName().Version.Revision}";
            return $"{typeof(Program).Assembly.GetName().Version.Major}.{typeof(Program).Assembly.GetName().Version.Minor}.{typeof(Program).Assembly.GetName().Version.Build}.{revision} beta";
        }
    }
}