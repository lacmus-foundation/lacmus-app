using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RescuerLaApp.ViewModels;

namespace RescuerLaApp.Views
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