using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using LacmusApp.Avalonia.ViewModels;

namespace LacmusApp.Avalonia.Views
{
    public class FirstWizardView : ReactiveUserControl<FirstWizardViewModel>
    {
        public FirstWizardView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}