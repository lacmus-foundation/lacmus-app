using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.Managers;
using LacmusApp.ViewModels;
using ReactiveUI;

namespace LacmusApp.Views
{
    public class MetadataWindow : ReactiveWindow<MetadataViewModel>
    {
        public MetadataWindow(ThemeManager themeManager)
        {
            var localThemeManager = new ThemeManager(this);
            localThemeManager.UseTheme(themeManager.CurrentTheme);
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
        public MetadataWindow() { }
    }
}