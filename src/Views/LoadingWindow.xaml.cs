using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.ViewModels;
using ReactiveUI;

namespace LacmusApp.Views
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