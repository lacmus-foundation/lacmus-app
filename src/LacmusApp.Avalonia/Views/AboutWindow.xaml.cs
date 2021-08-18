using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Avalonia.ViewModels;
using ReactiveUI;

namespace LacmusApp.Avalonia.Views
{
    public class AboutWindow : ReactiveWindow<AboutViewModel>
    {
        public AboutWindow(ThemeManager themeManager)
        {
            var localThemeManager = new ThemeManager(this);
            localThemeManager.UseTheme(themeManager.CurrentTheme);
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
        public AboutWindow() { }
    }
}