using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.Managers;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json;
using ReactiveUI;
using LacmusApp.Models;
using LacmusApp.ViewModels;

namespace LacmusApp.Views
{
    public sealed class WizardWindow : ReactiveWindow<WizardWindowViewModel>
    {
        public WizardWindow(ThemeManager themeManager)
        {
            var localThemeManager = new ThemeManager(this);
            localThemeManager.UseTheme(themeManager.CurrentTheme);
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
        public WizardWindow() { }
    }
}