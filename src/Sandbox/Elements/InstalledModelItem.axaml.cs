using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sandbox.Elements
{
    public class InstalledModelItem : UserControl
    {
        public InstalledModelItem()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}