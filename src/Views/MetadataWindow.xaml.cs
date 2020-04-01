using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.ViewModels;
using ReactiveUI;

namespace LacmusApp.Views
{
    public class MetadataWindow : ReactiveWindow<MetadataViewModel>
    {
        public MetadataWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}