using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LacmusApp.Avalonia.Elements
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