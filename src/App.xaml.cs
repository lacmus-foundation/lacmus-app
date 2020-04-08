using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LacmusApp.Managers;
using LacmusApp.ViewModels;
using LacmusApp.Views;

namespace LacmusApp
{
    public class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                var window = new MainWindow();
                window.DataContext = new MainWindowViewModel(window, new ThemeManager(window));
                desktopLifetime.MainWindow = window;
            }
            
            base.OnFrameworkInitializationCompleted();
        }
    }
}
