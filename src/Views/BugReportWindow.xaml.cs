using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.Managers;
using LacmusApp.ViewModels;
using ReactiveUI;

namespace LacmusApp.Views
{
    public class BugReportWindow : ReactiveWindow<BugReportViewModel>
    {
        public BugReportWindow(ThemeManager themeManager)
        {
            var localThemeManager = new ThemeManager(this);
            localThemeManager.UseTheme(themeManager.CurrentTheme);
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
        public BugReportWindow() { }
    }
}