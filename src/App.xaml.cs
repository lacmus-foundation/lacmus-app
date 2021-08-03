using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LacmusApp.Managers;
using LacmusApp.Models;
using LacmusApp.ViewModels;
using LacmusApp.Views;
using Serilog;

namespace LacmusApp
{
    public class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var window = new LoadingWindow();
                window.DataContext = new LoadingWindowViewModel(window);
                desktopLifetime.MainWindow = window;
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
