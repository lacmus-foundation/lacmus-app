using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
        public SettingsWindow(LocalizationContext context)
        {
            AvaloniaXamlLoader.Load(this);
            this.DataContext = new SettingsWindowViewModel(context);
        }

        public SettingsWindow() { }
    }
}