using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using LacmusApp.ViewModels;

namespace LacmusApp.Views
{
    public class FourthWizardView : ReactiveUserControl<FourthWizardViewModel>
    {
        public FourthWizardView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}