using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.Screens.Interfaces;
using ReactiveUI;

namespace LacmusApp.Avalonia.Views
{
    public class Settings : ReactiveWindow<ISettingsViewModel>
    {
        public Settings()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(context => context != null)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, x => x.ViewModel.Plugin.Activate)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(context => context != null)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, x => x.ViewModel.LocalPluginRepository.Refresh)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(context => context != null)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, x => x.ViewModel.RemotePluginRepository.Refresh)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}