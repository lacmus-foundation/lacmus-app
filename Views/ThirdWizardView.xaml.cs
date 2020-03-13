using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RescuerLaApp.ViewModels;

namespace RescuerLaApp.Views
{
    public class ThirdWizardView : ReactiveUserControl<ThirdWizardViewModel>
    {
        public ThirdWizardView()
        {
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }
}