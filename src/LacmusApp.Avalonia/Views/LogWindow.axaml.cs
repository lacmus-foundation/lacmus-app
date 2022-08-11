using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Screens.Interfaces;

namespace LacmusApp.Avalonia.Views;

public partial class LogWindow : Window
{
    public LogWindow()
    {
        InitializeComponent();
    }
    public LogWindow(ILogViewModel viewModel, ThemeManager themeManager)
    {
        var localThemeManager = new ThemeManager(this);
        localThemeManager.UseTheme(themeManager.CurrentTheme);
        InitializeComponent();
        this.DataContext = viewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}