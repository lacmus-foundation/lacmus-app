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
        public ModelManagerWindow(LocalizationContext context, AppConfig appConfig, ApplicationStatusManager manager, ThemeManager themeManager)
        {
            AvaloniaXamlLoader.Load(this);
            var localThemeManager = new ThemeManager(this);
            localThemeManager.UseTheme(themeManager.CurrentTheme);
            this.DataContext = new ModelManagerWindowViewModel(context, appConfig, manager);
        }
        public ModelManagerWindow() { }
    }
}