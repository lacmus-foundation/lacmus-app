using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Services;
using LacmusApp.Avalonia.Services.Files;
using LacmusApp.Avalonia.ViewModels;

namespace LacmusApp.Avalonia.Views
{
    public class ModelManagerWindow : Window
    {
        public AppConfig AppConfig { get; set; }
        public ModelManagerWindow(LocalizationContext context, ref AppConfig appConfig, ApplicationStatusManager manager, ThemeManager themeManager)
        {
            AppConfig = appConfig;
            var localThemeManager = new ThemeManager(this);
            localThemeManager.UseTheme(themeManager.CurrentTheme);
            AvaloniaXamlLoader.Load(this);
            this.DataContext = new ModelManagerWindowViewModel(this, context, ref appConfig, manager);
        }
        public ModelManagerWindow() { }

        public Task<AppConfig> ShowResult()
        {
            var tcs = new TaskCompletionSource<AppConfig>();
            Closed += delegate { tcs.TrySetResult(AppConfig); };
            Show();
            return tcs.Task;
        }
    }
}