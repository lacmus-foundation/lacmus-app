﻿using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.ViewModels;

namespace LacmusApp.Avalonia.Views
{
    public sealed class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ViewModel)
                    .Where(context => context != null)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(this, x => x.ViewModel.SettingsViewModel.Plugin.Activate)
                    .DisposeWith(disposables);
            });
            Zoomer.Init(this.Find<ZoomBorder>("zoomBorder"));
            Zoomer.KeyDown += ZoomBorder_KeyDown;
        }

        private void ZoomBorder_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.R:
                    Zoomer.Reset();
                    break;
                case Key.Up:
                    Zoomer.MoveTo(0, 25);
                    break;
                case Key.Down:
                    Zoomer.MoveTo(0, -25);
                    break;
                case Key.Left:
                    Zoomer.MoveTo(25, 0);
                    break;
                case Key.Right:
                    Zoomer.MoveTo(-25, 0);
                    break;
            }
        }
    }
}
