using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Services;
using LacmusApp.Avalonia.Services.IO;
using LacmusApp.Image.Enums;
using LacmusApp.Image.Models;
using LacmusApp.Image.Services;
using LacmusApp.Screens.ViewModels;
using LacmusPlugin;
using MetadataExtractor;
using Serilog;
using Directory = System.IO.Directory;
using Object = LacmusApp.Image.Models.Object;


namespace LacmusApp.Avalonia.ViewModels
{
    public class FourthWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly ApplicationStatusManager _applicationStatusManager;
        private readonly SourceList<PhotoViewModel> _photos;
        private SettingsViewModel _settingsViewModel;
        private int _selectedIndex;
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        [Reactive] public LocalizationContext LocalizationContext { get; set; }
        [Reactive] public double InputProgress { get; set; }
        [Reactive] public double PredictProgress { get; set; }
        [Reactive] public double OutputProgress { get; set; }
        [Reactive] public string InputTextProgress { get; set; } = "waiting...";
        [Reactive] public string PredictTextProgress { get; set; } = "waiting...";
        [Reactive] public string OutputTextProgress { get; set; } = "waiting...";
        [Reactive] public string Status { get; set; } = "";
        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        public FourthWizardViewModel(IScreen screen, 
            SettingsViewModel settingsViewModel,
            ApplicationStatusManager manager,
            SourceList<PhotoViewModel> photos,
            int selectedIndex, LocalizationContext localizationContext)
        {
            _applicationStatusManager = manager;
            _photos = photos;
            _selectedIndex = selectedIndex;
            _settingsViewModel = settingsViewModel;
            LocalizationContext = localizationContext;
            StopCommand = ReactiveCommand.Create(Stop);
            HostScreen = screen;
        }
        
        public async Task OpenFile(string inputPath)
        {
            try
            {
                Status = "loading photos...";
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                using (var pb = new Models.ProgressBar())
                {
                    var count = 0;
                    var id = 0;
                    var files = GetFilesFromDir(inputPath, false);
                    files = files.Where(s =>
                        s.ToLower().EndsWith(".png") ||
                        s.ToLower().EndsWith(".jpg") ||
                        s.ToLower().EndsWith(".jpeg"));
                    var reader = new AvaloniaBrushReader(LoadType.Miniature);
                    var enumerable = files as string[] ?? files.ToArray();
                    _photos.Clear();
                    foreach (var path in enumerable)
                    {
                        try
                        {
                            await using (var stream = File.OpenRead(path))
                            {
                                var (brush, height, width) = await reader.Read(stream);
                                var (metadata, latitude, longitude, altitude) = ExifConvertor.ConvertExif(
                                    ImageMetadataReader.ReadMetadata(path));
                                var photoViewModel = new PhotoViewModel(id)
                                {
                                    Brush = brush,
                                    Detections = new List<IObject>(),
                                    Height = height,
                                    Width = width,
                                    Path = path,
                                    Latitude = latitude,
                                    Longitude = longitude,
                                    Altitude = altitude,
                                    ExifDataCollection = metadata
                                };
                                
                                _photos.Add(photoViewModel);
                                id++;
                                count++;
                                InputProgress = (double)count / enumerable.Count() * 100;
                                InputTextProgress = $"{Convert.ToInt32(InputProgress)} %";
                                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, $"Working | {(int)((double) count / enumerable.Count() * 100)} %, [{count} of {enumerable.Count()}]");
                                pb.Report((double)count / enumerable.Count(), $"Processed {count} of {enumerable.Count()}");
                            }
                        }
                        catch (Exception e)
                        { 
                            Log.Warning(e,$"image from {path} is skipped!");
                        }
                    }
                }
                _selectedIndex = 0;
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
                InputTextProgress = $"loads {_photos.Count} photos.";
                Log.Information($"Loads {_photos.Count} photos.");
            }
            catch (Exception ex)
            {
                Status = "error.";
                Log.Error(ex,"Unable to load photos.");
            }
        }
        
        public async Task PredictAll()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
            try
            {
                Status = "starting ml model...";
                var plugin = _settingsViewModel.Plugin;
                if (string.IsNullOrEmpty(plugin.Tag))
                    throw new Exception("No such plugin");
                
                using (var model = plugin.LoadModel(_settingsViewModel.PredictionThreshold))
                {
                    var count = 0;
                    var objectCount = 0;
                    Status = "processing...";
                    foreach (var photoViewModel in _photos.Items)
                    {
                        try
                        {
                            var detections = await Dispatcher.UIThread.InvokeAsync( () =>
                                Task.Run(() =>  model.Infer(photoViewModel.Path,
                                    photoViewModel.Width,
                                    photoViewModel.Height)));
                            var enumerable = detections as IObject[] ?? detections.ToArray();
                            photoViewModel.Detections = enumerable;
                            objectCount += photoViewModel.BoundBoxes.Count();
                            count++;
                            PredictProgress = (double) count / _photos.Items.Count() * 100;
                            PredictTextProgress = $"{Convert.ToInt32(PredictProgress)} %";
                            Console.WriteLine($"\tProgress: {(double) count / _photos.Items.Count() * 100} %");
                            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, $"Working | {(int)((double) count / _photos.Items.Count() * 100)} %, [{count} of {_photos.Items.Count()}]");
                        }
                        catch (Exception e)
                        {
                            Log.Error(e,$"Unable to process file {photoViewModel.Path}. Slipped.");
                        }
                    }
                    Log.Information($"Successfully predict {_photos.Items.Count()} photos. Find {objectCount} objects.");
                }
            }
            catch (Exception e)
            {
                Status = "error.";
                Log.Error(e, "Unable to get prediction.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
        
        public async Task SaveAll(SecondWizardViewModel viewModel)
        {
            try
            {
                if (!_photos.Items.Any())
                {
                    Log.Warning("There are no photos to save.");
                    OutputTextProgress = $"no photos to save.";
                    Status = "done.";
                    return;
                }
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                Status = "saving results...";
                try
                {
                    var photosToSave = Filter(_photos.Items, viewModel.FilterIndex);
                    var viewModels = photosToSave as PhotoViewModel[] ?? photosToSave.ToArray();
                    var saver = new PhotoSaver(new Window());
                    var saveParams = new SaveAsParams()
                    {
                        SaveImage = viewModel.IsSaveImage,
                        SaveXml = viewModel.IsSaveXml,
                        SaveCrop = viewModel.IsSaveCrop,
                        SaveDrawImage = viewModel.IsSaveDrawImage,
                        SaveGeoPosition = viewModel.IsSaveGeoPosition
                    };
                    await saver.SaveAs(saveParams, viewModels, viewModel.OutputPath);
                    OutputProgress = 100.0;
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to save photo!",e);
                }
                Log.Information($"Saved {_photos.Count} photos.");
            }
            catch (Exception ex)
            {
                Status = "error.";
                Log.Error(ex, "Unable to save photos.");
            }
            Status = "done.";
            OutputTextProgress = $"saved {_photos.Count} photos.";
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private async void Stop()
        {
            //TODO: cancel long operations
        }
        private static IEnumerable<string> GetFilesFromDir(string dirPath, bool isRecursive)
        {
            return Directory.GetFiles(dirPath, "*.*",
                isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
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