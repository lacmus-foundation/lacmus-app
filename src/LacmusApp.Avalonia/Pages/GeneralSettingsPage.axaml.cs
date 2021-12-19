using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LacmusApp.Avalonia.Pages
{
    public class GeneralSettingsPage : UserControl
    {
        public GeneralSettingsPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}