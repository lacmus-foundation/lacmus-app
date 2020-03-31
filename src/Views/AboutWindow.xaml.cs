using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.ViewModels;
using ReactiveUI;

namespace LacmusApp.Views
{
    public class AboutWindow : ReactiveWindow<AboutViewModel>
    {
        public AboutWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}