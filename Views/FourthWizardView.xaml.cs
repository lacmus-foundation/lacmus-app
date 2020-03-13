using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RescuerLaApp.ViewModels;

namespace RescuerLaApp.Views
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