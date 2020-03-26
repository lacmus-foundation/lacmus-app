using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RescuerLaApp.Extensions;
using RescuerLaApp.Managers;
using RescuerLaApp.Models;
using RescuerLaApp.Models.ML;
using RescuerLaApp.Models.Photo;
using RescuerLaApp.Services.IO;
using RescuerLaApp.Services.VM;
using Serilog;
using Attribute = RescuerLaApp.Models.Photo.Attribute;

namespace RescuerLaApp.ViewModels
{
    public class FourthWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly string _mlConfigPath = Path.Join("conf", "mlConfig.json");
        private readonly ApplicationStatusManager _applicationStatusManager;
        private readonly SourceList<PhotoViewModel> _photos;
        private int _selectedIndex;
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        [Reactive] public double InputProgress { get; set; }
        [Reactive] public double PredictProgress { get; set; }
        [Reactive] public double OutputProgress { get; set; }
        [Reactive] public string InputTextProgress { get; set; } = "waiting...";
        [Reactive] public string PredictTextProgress { get; set; } = "waiting...";
        [Reactive] public string OutputTextProgress { get; set; } = "waiting...";
        [Reactive] public string Status { get; set; } = "";
        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        public FourthWizardViewModel(IScreen screen, 
            ApplicationStatusManager manager,
            SourceList<PhotoViewModel> photos,
            int selectedIndex)
        {
            _applicationStatusManager = manager;
            _photos = photos;
            _selectedIndex = selectedIndex;
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
                    var photoLoader = new PhotoLoader();
                    var files = GetFilesFromDir(inputPath, false);
                    var enumerable = files as string[] ?? files.ToArray();
                    _photos.Clear();
                    foreach (var path in enumerable)
                    {
                        try
                        {
                            await using (var stream = File.OpenRead(path))
                            {
                                if (Path.GetExtension(path).ToLower() != ".jpg" &&
                                        Path.GetExtension(path).ToLower() != ".jpeg" &&
                                        Path.GetExtension(path).ToLower() != ".png")
                                {
                                    count++;
                                    continue;
                                }
                                var annotation = new Annotation
                                {
                                    Filename = Path.GetFileName(path),
                                    Folder = Path.GetDirectoryName(path)
                                };
                                    
                                await Task.Run(async () =>
                                {
                                    await Dispatcher.UIThread.InvokeAsync(() =>
                                    {
                                        var photo = photoLoader.Load(path, stream, PhotoLoadType.Miniature);
                                        _photos.Add(new PhotoViewModel(id, photo, annotation));
                                        id++;
                                    });
                                });
                                
                                count++;
                                InputProgress = (double)count / enumerable.Count() * 100;
                                InputTextProgress = $"{Convert.ToInt32(InputProgress)} %";
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
                var config = await MLModelConfigExtension.Load(_mlConfigPath);
                using (var model = new MLModel(config))
                {
                    await model.Init();
                    var count = 0;
                    var objectCount = 0;
                    Status = "processing...";
                    foreach (var photoViewModel in _photos.Items)
                    {
                        try
                        {
                            photoViewModel.Annotation.Objects = await model.Predict(photoViewModel);
                            photoViewModel.BoundBoxes = photoViewModel.GetBoundingBoxes();
                            if (photoViewModel.BoundBoxes.Any())
                            {
                                photoViewModel.Photo.Attribute = Attribute.WithObject;
                                photoViewModel.IsHasObject = true;
                            }
                            objectCount += photoViewModel.BoundBoxes.Count();
                            count++;
                            PredictProgress = (double) count / _photos.Items.Count() * 100;
                            PredictTextProgress = $"{Convert.ToInt32(PredictProgress)} %";
                            Console.WriteLine($"\tProgress: {(double) count / _photos.Items.Count() * 100} %");
                        }
                        catch (Exception e)
                        {
                            Log.Error(e,$"Unable to process file {photoViewModel.Path}. Slipped.");
                        }
                    }
                    Status = "stopping ml model...";
                    await model.Stop();
                    PredictTextProgress = $"predict {_photos.Count} photos.";
                    Log.Information($"Successfully predict {_photos.Count} photos. Find {objectCount} objects.");
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
                            await Task.Run(() =>
                            {
                                var srcPhotoPath = photoViewModel.Path;
                                var dstPhotoPath = Path.Combine(outputPath, photoViewModel.Annotation.Filename);
                                var annotationPath = Path.Combine(outputPath, $"{photoViewModel.Annotation.Filename}.xml");
                                var annotation = photoViewModel.Annotation;
                                annotation.Folder = outputPath;
                                var saver = new AnnotationSaver();
                                saver.Save(annotation, annotationPath);
                                if (srcPhotoPath == dstPhotoPath)
                                {
                                    Log.Warning($"Photo {srcPhotoPath} skipped. File exists.");
                                    count++;
                                    OutputProgress = (double) count / viewModels.Count() * 100;
                                    OutputTextProgress = $"{Convert.ToInt32(OutputProgress)} %";
                                    pb.Report((double)count / viewModels.Count(), $"Saving files {count} of {viewModels.Length}");
                                    return;
                                }
                                File.Copy(srcPhotoPath, dstPhotoPath, true);
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
            var config = await MLModelConfigExtension.Load(_mlConfigPath);
            using (var model = new MLModel(config))
            {
                await model.Stop();
            }
        }
        private static IEnumerable<string> GetFilesFromDir(string dirPath, bool isRecursive)
        {
            return Directory.GetFiles(dirPath, "*.*",
                isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
    }
}