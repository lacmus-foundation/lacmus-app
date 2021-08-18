using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LacmusApp.Avalonia.Managers;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Services;
using LacmusApp.Avalonia.ViewModels;
using LacmusApp.Avalonia.Services.Files;


namespace LacmusApp.Avalonia.Views
{
    public class SettingsWindow : Window
    {
        public AppConfig AppConfig { get; set; }
        public SettingsWindow(LocalizationContext context, ref AppConfig appConfig, ApplicationStatusManager manager, ThemeManager themeManager)
        {
            AppConfig = appConfig;
            AvaloniaXamlLoader.Load(this);
            var settingsThemeManager = new ThemeManager(this);
            settingsThemeManager.UseTheme(themeManager.CurrentTheme);
            this.DataContext = new SettingsWindowViewModel(this, context, ref appConfig, manager, themeManager);
        }

        public SettingsWindow() { }
        public Task<AppConfig> ShowResult()
        {
            var tcs = new TaskCompletionSource<AppConfig>();
            Closed += delegate { tcs.TrySetResult(AppConfig); };
            Show();
            return tcs.Task;
        }
    }
}