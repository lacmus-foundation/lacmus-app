using System;
using System.IO;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace RescuerLaApp.ViewModels
{
    public class FirstWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        private string _inputPath;
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public ReactiveCommand<Unit, Unit> OpenPhotos { get; }

        [Reactive]
        public string InputPath
        {
            get => _inputPath;
            set
            {
                _inputPath = value;
                Log.Debug(_inputPath);
            }
        }

        public FirstWizardViewModel(IScreen screen)
        {
            HostScreen = screen;
            OpenPhotos = ReactiveCommand.Create(Open);
        }

        private async void Open()
        {
            try
            {
                var dig = new OpenFolderDialog()
                {
                    //TODO: Multi language support
                    Title = "Chose directory image files"
                };
                var dirPath = await dig.ShowAsync(new Window());
                InputPath = dirPath;
            }
            catch (Exception e)
            {
                Log.Error("Unable to setup input path.", e);
            }
            
        }
    }
}