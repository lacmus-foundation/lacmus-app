using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using LacmusApp.ViewModels;

namespace LacmusApp.Views
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