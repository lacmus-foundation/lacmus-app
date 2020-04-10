using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LacmusApp.Managers;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json;
using LacmusApp.Models;
using LacmusApp.ViewModels;
using LacmusApp.Services.Files;


namespace LacmusApp.Views
{
    class SettingsWindow : Window
    {
        public SettingsWindow(LocalizationContext context, AppConfig appConfig, ThemeManager themeManager)
        {
            AvaloniaXamlLoader.Load(this);
            var manager = new ThemeManager(this);
            manager.UseTheme(themeManager.CurrentTheme);
            this.DataContext = new SettingsWindowViewModel(context, appConfig, themeManager, manager);
        }

        public SettingsWindow() { }
    }
}