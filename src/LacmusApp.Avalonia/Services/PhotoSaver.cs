using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Controls;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Services.Files;
using LacmusApp.Avalonia.ViewModels;
using LacmusApp.Image.Models;
using Serilog;
using SkiaSharp;
using Object = LacmusApp.Image.Models.Object;
using ProgressBar = LacmusApp.Avalonia.Models.ProgressBar;

namespace LacmusApp.Avalonia.Services;

public class PhotoSaver
{
    private readonly IAvaloniaFileSelector _writer;
    
    public PhotoSaver(Window window)  => _writer = new AvaloniaFileSelector(window);
    
    public delegate void StstusHandler(Enums.Status status, string statusString);
    public event StstusHandler Notify;

    public async Task Save(IEnumerable<PhotoViewModel> photos)
    {
        await SaveAs(new SaveAsParams() {SaveImage = true, SaveXml = true}, photos);
    }

    public async Task SaveAs(SaveAsParams saveParams, IEnumerable<PhotoViewModel> photos, string dir = null)
    {
        if (dir == null)
        {
            var dig = new OpenFolderDialog()
            {
                //TODO: Multi language support
                Title = "Chose directory to save files"
            };
            dir = await _writer.SelectDir(dig);
        }
        var count = 0;
        
        using (var pb = new ProgressBar())
        {
            var photoViewModels = photos as PhotoViewModel[] ?? photos.ToArray();
            await Parallel.ForEachAsync(photoViewModels, async (photo, _) =>
            {
                try
                {
                    if (saveParams.SaveXml)
                        await SaveXml(photo, dir);

                    if (saveParams.SaveCrop)
                        await SaveCrops(photo, dir);

                    if (saveParams.SaveImage)
                        await SaveImage(photo, dir);

                    if (saveParams.SaveDrawImage)
                        await SaveDrawImage(photo, dir);

                    if (saveParams.SaveGeoPosition)
                        await SaveGeoPosition(photo, dir);

                    count++;
                    Notify?.Invoke(Enums.Status.Working,
                        $"Working | {(int)((double)count / photoViewModels.Length * 100)} %, [{count} of {photoViewModels.Length}]");
                    pb.Report((double)count / photoViewModels.Length, $"Processed {count} of {photoViewModels.Length}");
                }
                catch (Exception e)
                {
                    Log.Warning(e, $"Image from {photo.Path} is skipped!");
                }
            });
        }
    }

    private async Task SaveGeoPosition(PhotoViewModel photoViewModel, string saveDir)
    {
        await Task.Run(async () =>
        {
            
            var path = Path.Combine(saveDir, 
                $"{Path.GetFileName(photoViewModel.Path)}_geo_position.txt");
            var lines = new []
            {
                $"{photoViewModel.Latitude};{photoViewModel.Longitude}",
                "------------------------------------",
                $"Latitude: {photoViewModel.Latitude}",
                $"Longitude: {photoViewModel.Latitude}",
            };
            await File.WriteAllLinesAsync(path, lines);
        });
    }
    
    private async Task SaveDrawImage(PhotoViewModel photoViewModel, string saveDir)
    {
        await Task.Run(async () =>
        {
            using (var bitmap = SKBitmap.Decode(photoViewModel.Path))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    var paint = new SKPaint {
                        Style = SKPaintStyle.Stroke,
                        Color = SKColors.Red,
                        StrokeWidth = 10
                    };
                    foreach (var detection in photoViewModel.Detections)
                    {
                        canvas.DrawRect(SKRect.Create(detection.XMin, detection.YMin, detection.Width, detection.Height), paint);
                    }
                    var encodedData = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
                    var stream = encodedData.AsStream();
                    var path = Path.Join(saveDir,
                        $"{Path.GetFileName(photoViewModel.Path)}__draw.png");
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }
            }
        });
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
    
    private async Task SaveCrops(PhotoViewModel photoViewModel, string saveDir)
    {
        await Task.Run(async () =>
        {
            using (var bitmap = SKBitmap.Decode(photoViewModel.Path))
            {
                var image = SKImage.FromBitmap(bitmap);
                var count = 0;
                foreach (var bbox in photoViewModel.Detections)
                {
                    var xMin = Math.Max(0, bbox.XMin - 100);
                    var xMax = Math.Min(photoViewModel.Width, bbox.XMax + 100);
                    var yMin = Math.Max(0, bbox.YMin - 100);
                    var yMax = Math.Min(photoViewModel.Height, bbox.YMax + 100);
                    var subset = image.Subset(new SKRectI(xMin, yMin, xMax, yMax));
                    var encodedData = subset.Encode(SKEncodedImageFormat.Png, 100);
                    var stream = encodedData.AsStream();
                    var path = Path.Join(saveDir,
                        $"{Path.GetFileName(photoViewModel.Path)}_crop{count}.png");
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                    count++;
                }
            }
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