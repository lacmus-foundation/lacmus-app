using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RescuerLaApp.Models;
using RescuerLaApp.ViewModels;

namespace RescuerLaApp.Views
{
    public sealed class WizardWindow : ReactiveWindow<MainWindowViewModel>
    {
        public WizardWindow()
        {
            AvaloniaXamlLoader.Load(this);
            this.WhenActivated(disposables => { });
        }
    }
}