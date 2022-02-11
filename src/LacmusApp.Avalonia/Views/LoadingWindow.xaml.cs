using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.Avalonia.ViewModels;
using ReactiveUI;

namespace LacmusApp.Avalonia.Views
{
    public class LoadingWindow : ReactiveWindow<LoadingWindowViewModel>
    {
        public LoadingWindow()
        {
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(context => context != null)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, x => x.ViewModel.InitCommand)
                    .DisposeWith(disposables);
            });
            AvaloniaXamlLoader.Load(this);
        }
    }
}