using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using DynamicData;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;

namespace LacmusApp.Avalonia.ViewModels
{
    public class SaveAsWindowViewModel : ReactiveValidationObject
    {
        private readonly SourceList<PhotoViewModel> _photos;
        private readonly ApplicationStatusManager _applicationStatusManager;
        public SaveAsWindowViewModel(Window window, SourceList<PhotoViewModel> photos, ApplicationStatusManager applicationStatusManager, LocalizationContext localizationContext)
        {
            LocalizationContext = localizationContext;
            IsSaveImage = true;
            IsSaveXml = true;
            
            _photos = photos;
            _applicationStatusManager = applicationStatusManager;
            this.ValidationRule(
                viewModel => viewModel.OutputPath,
                Directory.Exists,
                path => $"Incorrect path {path}");

            SelectPathCommand = ReactiveCommand.Create(SelectOutputFolder);
            SaveCommand = ReactiveCommand.CreateFromTask(SavePhotos, this.IsValid());
        }
        [Reactive] public string OutputPath { get; set; }
        [Reactive] public int FilterIndex { get; set; } = 0;
        [Reactive] public bool IsSaveCrop { get; set; }
        [Reactive] public bool IsSaveXml { get; set; }
        [Reactive] public bool IsSaveImage { get; set; }
        [Reactive] public bool IsSaveDrawImage { get; set; }
        [Reactive] public bool IsSaveGeoPosition { get; set; }
        [Reactive] public LocalizationContext LocalizationContext { get; set; }
        public ReactiveCommand<Unit, Unit> SelectPathCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }

        private async void SelectOutputFolder()
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
                Console.WriteLine(e);
                throw;
            }
        }
        
        private async Task SavePhotos()
        {
            try
            {
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                if(!Directory.Exists(OutputPath))
                    throw new Exception($"No such directory: {OutputPath}.");
                    
                //select filter
                var photosToSave = Filter(_photos.Items, FilterIndex);
                var photoViewModels = photosToSave as PhotoViewModel[] ?? photosToSave.ToArray();
                if (!photoViewModels.Any())
                {
                    Log.Warning("There are no photos to save.");
                    _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
                    return;
                }

                var photoSaver = new PhotoSaver(new Window());
                var saveParams = new SaveAsParams()
                {
                    SaveCrop = IsSaveCrop,
                    SaveImage = IsSaveImage,
                    SaveXml = IsSaveXml,
                    SaveDrawImage = IsSaveDrawImage,
                    SaveGeoPosition = IsSaveGeoPosition
                };
                await photoSaver.SaveAs(saveParams, photoViewModels, OutputPath);
                
                Log.Information($"Saved {photoViewModels.Length} photos.");
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
            }
            catch (Exception e)
            {
                Log.Error("Unable to save photos.", e);
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
            }
        }
        
        private IEnumerable<PhotoViewModel> Filter(IEnumerable<PhotoViewModel> sourceList, int fitlerType)
        {
            List<PhotoViewModel> resList = new List<PhotoViewModel>();
            foreach (var item in sourceList)
            {
                switch (fitlerType)
                {
                    case 0:
                        resList.Add(item);
                        break;
                    case 1:
                        if(item.IsHasObjects)
                            resList.Add(item);
                        break;
                    case 2:
                        if(item.IsFavorite)
                            resList.Add(item);
                        break;
                    default:
                        throw new Exception($"invalid filter index {fitlerType}");
                }
            }

            return resList;
        }
    }
}