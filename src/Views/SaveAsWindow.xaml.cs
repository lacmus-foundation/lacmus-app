using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.ViewModels;
using ReactiveUI;

namespace LacmusApp.Views
{
    public class SaveAsWindow : ReactiveWindow<SaveAsWindowViewModel>
    {
        public SaveAsWindow()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}