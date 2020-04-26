using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LacmusApp.Managers;
using LacmusApp.Models;
using LacmusApp.Services.Files;
using LacmusApp.ViewModels;

namespace LacmusApp.Views
{
    public class ModelManagerWindow : Window
    {
        public AppConfig AppConfig { get; set; }
        public ModelManagerWindow(LocalizationContext context, ref AppConfig appConfig, ApplicationStatusManager manager, ThemeManager themeManager)
        {
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