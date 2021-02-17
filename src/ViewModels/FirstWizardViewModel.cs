using System;
using System.IO;
using System.Reactive;
using Avalonia.Controls;
using LacmusApp.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class FirstWizardViewModel : ReactiveValidationObject<FirstWizardViewModel>, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        public ReactiveCommand<Unit, Unit> OpenPhotos { get; }

        [Reactive] public string InputPath { get; set; }
        [Reactive] public LocalizationContext LocalizationContext { get; set; }

        public FirstWizardViewModel(IScreen screen, LocalizationContext localizationContext)
        {
            HostScreen = screen;
            LocalizationContext = localizationContext;
            
            this.ValidationRule(
                viewModel => viewModel.InputPath,
                Directory.Exists,
                path => $"Incorrect path {path}");
            
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