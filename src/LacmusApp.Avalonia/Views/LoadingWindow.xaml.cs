using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.Avalonia.ViewModels;
using ReactiveUI;

namespace LacmusApp.Avalonia.Views
{
    public class LoadingWindow : ReactiveWindow<LoadingWindowViewModel>
    {
        public LoadingWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}