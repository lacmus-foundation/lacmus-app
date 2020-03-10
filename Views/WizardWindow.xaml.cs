using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json;
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

        public void Show(MainWindowViewModel vm)
        {
            DataContext = vm;
            base.Show();
        }
    }
}