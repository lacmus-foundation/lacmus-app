using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sandbox.Elements
{
    public class RemoteModelItem : UserControl
    {
        public RemoteModelItem()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}