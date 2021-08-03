using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using LacmusApp.ViewModels;

namespace LacmusApp.Views
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