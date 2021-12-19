using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LacmusApp.Avalonia.Pages
{
    public class InstalledPluginsPage : UserControl
    {
        public InstalledPluginsPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}