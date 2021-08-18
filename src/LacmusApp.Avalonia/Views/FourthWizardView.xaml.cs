using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using LacmusApp.Avalonia.ViewModels;

namespace LacmusApp.Avalonia.Views
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