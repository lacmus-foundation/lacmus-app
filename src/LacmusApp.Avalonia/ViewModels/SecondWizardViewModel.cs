using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using LacmusApp.Avalonia.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;

namespace LacmusApp.Avalonia.ViewModels
{
    public class SecondWizardViewModel : ReactiveValidationObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public ReactiveCommand<Unit, Unit> SavePhotos { get; }

        [Reactive] public string OutputPath { get; set; }
        [Reactive] public int FilterIndex { get; set; } = 0;
        [Reactive] public bool IsSaveCrop { get; set; }
        [Reactive] public bool IsSaveXml { get; set; }
        [Reactive] public bool IsSaveImage { get; set; }
        [Reactive] public bool IsSaveDrawImage { get; set; }
        [Reactive] public bool IsSaveGeoPosition { get; set; }
        [Reactive] public LocalizationContext LocalizationContext { get; set; }

        public SecondWizardViewModel(IScreen screen, LocalizationContext localizationContext)
        {
            IsSaveXml = true;
            IsSaveImage = true;
            HostScreen = screen;
            LocalizationContext = localizationContext;
            this.ValidationRule(
                viewModel => viewModel.OutputPath,
                Directory.Exists,
                path => $"Incorrect path {path}");
            
            SavePhotos = ReactiveCommand.CreateFromTask(Save);
        }
        private async Task Save()
        {
            try
            {
                var dig = new OpenFolderDialog()
                {
                    //TODO: Multi language support
                    Title = "Select folder to save"
                };
                var dirPath = await dig.ShowAsync(new Window());
                OutputPath = dirPath;
            }
            catch (Exception e)
            {
                Log.Error("Unable to setup input path.", e);
            }
        }
    }
}