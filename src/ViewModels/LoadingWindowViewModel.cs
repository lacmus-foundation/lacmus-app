using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using LacmusApp.Models;
using LacmusApp.Views;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace LacmusApp.ViewModels
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
            var config = await LoadConfig();
            window.DataContext = new MainWindowViewModel(window, config);
            window.Show();
            _window.Close();
        }
        
        private async Task<AppConfig> LoadConfig()
        {
            var confDir = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "conf");
            var configPath = Path.Join(confDir,"appConfig.json");
            Console.WriteLine(configPath);
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
            return $"{typeof(Program).Assembly.GetName().Version.Major}.{typeof(Program).Assembly.GetName().Version.Minor}.{typeof(Program).Assembly.GetName().Version.Build}.{revision} alpha";
        }
    }
}