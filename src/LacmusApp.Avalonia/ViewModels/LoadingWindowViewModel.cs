using System;
using System.Globalization;
using System.IO;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia.Controls;
using LacmusApp.Appearance.Enums;
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
        }
        
        [Reactive] public string TextVersion { get; set; } = GetVersion() + ".";
        public ReactiveCommand<Unit, Unit> InitCommand { get; set; }

        private async void Init()
        {
            var logModel = new LogViewModel();
            var logPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lacmus", "log.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .WriteTo.Sink(logModel)
                .CreateLogger();
            
            var confDir = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                "lacmus");
            var configPath = Path.Join(confDir, "appConfig-v3.json");
            var configManager = new ConfigManager(configPath);
            var config = await configManager.ReadConfig();

            switch (config.Language)
            {
                case Language.English:
                    Properties.Settings.Culture = new CultureInfo("en");
                    break;
                case Language.Russian:
                    Properties.Settings.Culture = new CultureInfo("ru");
                    break;
                default:
                    Properties.Settings.Culture = new CultureInfo("en");
                    break;
            }
            
            var window = new MainWindow();
            
            await Task.Delay(1000);
            var dialog = new AvaloniaPluginDialog(window);
            var pluginManager = new PluginManager(
                Path.Join(confDir, "plugins"), "http://api.lacmus.ml");
            var themeManager = new ThemeManager(window);
            themeManager.UseTheme(config.Theme);
            var settingsViewModel = new SettingsViewModel(
                config,
                configManager,
                pluginManager,
                dialog);
            
            window.DataContext = new MainWindowViewModel(
                window,
                logModel,
                settingsViewModel,
                themeManager);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var logWindow = new LogWindow(logModel, themeManager);
                logWindow.Show();
            }
            
            window.Show();
            window.Closing += (sender, args) => Environment.Exit(0);
            
            _window.Close();
        }
        
        private static string GetVersion()
        {
            var revision = "";
            if (typeof(Program).Assembly.GetName().Version.Revision != 0)
                revision = $"preview-{typeof(Program).Assembly.GetName().Version.Revision}";
            return $"{typeof(Program).Assembly.GetName().Version.Major}.{typeof(Program).Assembly.GetName().Version.Minor}.{typeof(Program).Assembly.GetName().Version.Build}.{revision}";
        }
    }
}