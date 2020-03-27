using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using LacmusApp.ViewModels;

namespace LacmusApp.Views
{
    public class SecondWizardView : ReactiveUserControl<SecondWizardViewModel>
    {
        public SecondWizardView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}