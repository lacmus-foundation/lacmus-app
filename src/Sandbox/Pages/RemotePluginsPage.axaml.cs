using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sandbox.Pages
{
    public class RemotePluginsPage : UserControl
    {
        public RemotePluginsPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}