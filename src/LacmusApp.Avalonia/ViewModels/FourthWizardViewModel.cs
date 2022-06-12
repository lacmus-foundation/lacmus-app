using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
                                var (metadata, latitude, longitude) = ExifConvertor.ConvertExif(
                                    ImageMetadataReader.ReadMetadata(stream));
                                var photoViewModel = new PhotoViewModel(id)
                                {
                                    Brush = brush,
                                    Detections = new List<IObject>(),
                                    Height = height,
                                    Width = width,
                                    Path = path,
                                    Latitude = latitude,
                                    Longitude = longitude,
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
                //load config
                var confDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lacmus");
                var configPath = Path.Join(confDir,"appConfig-v2.json");
                
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
        
        public async Task SaveAll(string outputPath)
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
                    var count = 0;
                    var viewModels = _photos.Items as PhotoViewModel[] ?? _photos.Items.ToArray();
                    using (var pb = new ProgressBar())
                    {
                        foreach (var photoViewModel in viewModels)
                            await Task.Run(async () =>
                            {
                                await SaveImage(photoViewModel, outputPath);
                                await SaveXml(photoViewModel, outputPath);
                                count++;
                                OutputProgress = (double) count / viewModels.Count() * 100;
                                OutputTextProgress = $"{Convert.ToInt32(OutputProgress)} %";
                                pb.Report((double)count / viewModels.Count(), $"Saving files {count} of {viewModels.Length}");
                            });
                    }
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
        
    
        private async Task SaveImage(PhotoViewModel photoViewModel, string saveDir)
        {
            await Task.Run(() =>
            {
                if (!File.Exists(photoViewModel.Path))
                    throw new Exception("Source file is not exists.");
                var path = Path.Combine(saveDir, Path.GetFileName(photoViewModel.Path));
                File.Copy(photoViewModel.Path, path, true);
            });
        }
    
        private async Task SaveXml(PhotoViewModel photoViewModel, string saveDir)
        {
            await Task.Run(() =>
            {
                var annotation = new Annotation()
                {
                    Filename = Path.GetFileName(photoViewModel.Path),
                    Folder = Path.GetDirectoryName(saveDir),
                    Size = new Size()
                    {
                        Depth = 3,
                        Height = photoViewModel.Height,
                        Width = photoViewModel.Width
                    },
                    Segmented = 0,
                    Source = new Source(),
                    Objects = photoViewModel.Detections.Select(
                        x => new Object()
                        {
                            Difficult = 0,
                            Name = "Pedestrian",
                            Truncated = 0,
                            Box = new Box()
                            {
                                Xmax = x.XMax,
                                Xmin = x.XMin,
                                Ymax = x.YMax,
                                Ymin = x.YMin
                            }
                        }).ToList()
                };
        
                var formatter = new XmlSerializer(type:typeof(Annotation));
                var fileName = Path.Combine(saveDir, annotation.Filename + ".xml");
                using (var stream = File.Create(fileName))
                {
                    formatter.Serialize(stream, annotation);
                }
            });
        }
    }
}