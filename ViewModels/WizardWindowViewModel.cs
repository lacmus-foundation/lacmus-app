using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RescuerLaApp.Managers;
using RescuerLaApp.Models;
using Serilog;

namespace RescuerLaApp.ViewModels
{
    public class WizardWindowViewModel : ReactiveObject
    {
        private readonly Window _window;

        public WizardWindowViewModel(Window window)
        {
            _window = window;
            // Add here newer commands
            OpenFileCommand = ReactiveCommand.Create(OpenFile);

            Log.Information("Wizard started.");
        }

        #region Public API
        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; set; }

        #endregion

        void OpenFile()
        {
            Log.Debug("open file");
        }
    }
}