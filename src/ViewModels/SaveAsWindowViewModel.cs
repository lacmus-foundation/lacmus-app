using System.IO;
using System.Reactive;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace LacmusApp.ViewModels
{
    public class SaveAsWindowViewModel : ReactiveValidationObject<SaveAsWindowViewModel>
    {
        private readonly SourceList<PhotoViewModel> _photos;
        public SaveAsWindowViewModel(Window window, SourceList<PhotoViewModel> photos)
        {
            _photos = photos;
            this.ValidationRule(
                viewModel => viewModel.OutputPath,
                Directory.Exists,
                path => $"Incorrect path {path}");
            
            SaveCommand = ReactiveCommand.Create(OpenLicense);
        }
        [Reactive] public string OutputPath { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
        
        public void OpenLicense()
        {
            
        }
    }
}