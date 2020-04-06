using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using LacmusApp.Managers;
using LacmusApp.Models;
using LacmusApp.Services.IO;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using SkiaSharp;
using Attribute = LacmusApp.Models.Photo.Attribute;

namespace LacmusApp.ViewModels
{
    public class SaveAsWindowViewModel : ReactiveValidationObject<SaveAsWindowViewModel>
    {
        private readonly SourceList<PhotoViewModel> _photos;
        private readonly ApplicationStatusManager _applicationStatusManager;
        public SaveAsWindowViewModel(Window window, SourceList<PhotoViewModel> photos, ApplicationStatusManager applicationStatusManager)
        {
            _photos = photos;
            _applicationStatusManager = applicationStatusManager;
            this.ValidationRule(
                viewModel => viewModel.OutputPath,
                Directory.Exists,
                path => $"Incorrect path {path}");
            
            SaveCommand = ReactiveCommand.Create(SavePhotos);
        }
        [Reactive] public string OutputPath { get; set; }
        [Reactive] public int FilterIndex { get; set; } = 0;
        [Reactive] public bool IsXml { get; set; } = true;
        [Reactive] public bool IsSource { get; set; } = true;
        [Reactive] public bool IsDraw { get; set; } = false;
        [Reactive] public bool IsCrop { get; set; } = false;
        public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
        
        public async void SavePhotos()
        {
            try
            {
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                var dig = new OpenFolderDialog()
                {
                    //TODO: Multi language support
                    Title = "Select folder to save"
                };
                var dirPath = await dig.ShowAsync(new Window());
                OutputPath = dirPath;
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

                var count = 0;
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, $"Working | {(int)((double) count / photoViewModels.Length * 100)} %, [{count} of {photoViewModels.Length}]");
                Parallel.ForEach(photoViewModels,  async photoViewModel =>
                {
                    await Task.Run(async () =>
                    {
                        if (IsSource)
                        {
                            var srcPhotoPath = photoViewModel.Path;
                            var dstPhotoPath = Path.Combine(OutputPath, photoViewModel.Annotation.Filename);
                            if (srcPhotoPath == dstPhotoPath)
                            {
                                Log.Warning($"Photo {srcPhotoPath} skipped. File exists.");
                                return;
                            }
                            File.Copy(srcPhotoPath, dstPhotoPath, true);
                        }
                    
                        if (IsXml)
                        {
                            var annotationPath = Path.Combine(OutputPath, $"{photoViewModel.Annotation.Filename}.xml");
                            var saver = new AnnotationSaver();
                            saver.Save(photoViewModel.Annotation, annotationPath);
                        }
                        
                        if(!IsDraw && !IsCrop)
                            return;

                        using var bitmap = SKBitmap.Decode(photoViewModel.Path);
                        {
                            if (IsCrop)
                            {
                                var image = SKImage.FromBitmap(bitmap);
                                var cropIdx = 0;
                                foreach (var bbox in photoViewModel.Annotation.Objects)
                                {
                                    var subset = image.Subset(new SKRectI(bbox.Box.Xmin, bbox.Box.Ymin, bbox.Box.Xmax, bbox.Box.Ymax));
                                    var encodedData = subset.Encode(SKEncodedImageFormat.Png, 100);
                                    var stream = encodedData.AsStream();
                                    var path = Path.Join(OutputPath,
                                        $"{photoViewModel.Annotation.Filename}_crop{cropIdx}.png");
                                    SaveStream(stream, path);
                                    cropIdx++;
                                }
                            }
                            if (IsDraw)
                            {
                                var canvas = new SKCanvas(bitmap);
                                var paint = new SKPaint {
                                    Style = SKPaintStyle.Stroke,
                                    Color = SKColors.Red,
                                    StrokeWidth = 10
                                };
                                foreach (var bbox in photoViewModel.Annotation.Objects)
                                {
                                    var x = bbox.Box.Xmin;
                                    var y = bbox.Box.Ymin;
                                    var width = bbox.Box.Xmax - bbox.Box.Xmin;
                                    var height = bbox.Box.Ymax - bbox.Box.Ymin;
                                    canvas.DrawRect(SKRect.Create(x, y, width, height), paint);
                                }
                            
                                var encodedData = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
                                var stream = encodedData.AsStream();
                                var path = Path.Join(OutputPath,
                                    $"{photoViewModel.Annotation.Filename}_draw.png");
                                SaveStream(stream, path);
                            }
                        }
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            count++;
                            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, $"Working | {(int)((double) count / photoViewModels.Length * 100)} %, [{count} of {photoViewModels.Length}]");
                            if (count >= photoViewModels.Length)
                                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
                        });
                    });
                });
                
                Log.Information($"Saved {photoViewModels.Length} photos.");
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
            }
            catch (Exception e)
            {
                Log.Error("Unable to save photos.", e);
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
            }
        }
        
        public void SaveStream(Stream stream, string destPath)
        {
            using (var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
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
                        if(item.Photo.Attribute == Attribute.WithObject)
                            resList.Add(item);
                        break;
                    case 2:
                        if(item.Photo.Attribute == Attribute.Favorite)
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