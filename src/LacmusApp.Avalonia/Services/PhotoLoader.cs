using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using LacmusApp.Avalonia.Extensions;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Services.Files;
using LacmusApp.Avalonia.Services.IO;
using LacmusApp.Avalonia.ViewModels;
using LacmusApp.Image.Enums;
using LacmusApp.Image.Models;
using LacmusApp.Image.Services;
using LacmusPlugin;
using MetadataExtractor;
using Serilog;

namespace LacmusApp.Avalonia.Services
{
    public class PhotoLoader
    {
        private readonly IAvaloniaFileSelector _reader;
        
        public PhotoLoader(Window window)  => _reader = new AvaloniaFileSelector(window);
        public delegate void StstusHandler(Enums.Status status, string statusString);
        public event StstusHandler Notify; 
        
        public async Task<PhotoViewModel[]> ReadAllFromDirByPhoto(LoadType loadType = LoadType.Miniature, bool isRecursive = false)
        {
            var dig = new OpenFolderDialog()
            {
                //TODO: Multi language support
                Title = "Chose directory image files"
            };
            var multipleFiles = await _reader.SelectAllFilesFromDir(dig, isRecursive);
            multipleFiles = multipleFiles.Where(s =>
                s.ToLower().EndsWith(".png") ||
                s.ToLower().EndsWith(".jpg") ||
                s.ToLower().EndsWith(".jpeg"));
            var reader = new AvaloniaBrushReader(loadType);
            var photoList = new List<PhotoViewModel>();
            var index = 0;
            using (var pb = new Models.ProgressBar())
            {
                var list = multipleFiles as string[] ?? multipleFiles.ToArray();
                foreach (var path in list)
                {
                    try
                    {
                        using (var stream = File.OpenRead(path))
                        {
                            var (brush, height, width) = await reader.Read(stream);
                            var (metadata, latitude, longitude) = ExifConvertor.ConvertExif(
                                ImageMetadataReader.ReadMetadata(path));
                            var photoViewModel = new PhotoViewModel(index)
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
                            photoList.Add(photoViewModel);
                            index++;
                            Notify?.Invoke(Enums.Status.Working, $"Working | {(int)((double) index / list.Length * 100)} %, [{index} of {list.Length}]");
                            pb.Report((double)index / list.Length, $"Processed {index} of {list.Length}");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e,$"image from {path} is skipped!");
                    }
                }
            }
            return photoList.ToArray();
        }
        
        public async Task<PhotoViewModel[]> ReadAllFromDirByAnnotation(LoadType loadType = LoadType.Miniature, bool isRecursive = false)
        {
            var dig = new OpenFolderDialog()
            {
                //TODO: Multi language support
                Title = "Chose directory image files"
            };
            var multipleFiles = await _reader.SelectAllFilesFromDir(dig, isRecursive);
            multipleFiles = multipleFiles.Where(s =>
                s.ToLower().EndsWith(".xml"));
            var reader = new AvaloniaBrushReader(loadType);
            var annotationLoader = new AnnotationLoader();
            var photoList = new List<PhotoViewModel>();
            var index = 0;
            var list = multipleFiles as string[] ?? multipleFiles.ToArray();
            using (var pb = new Models.ProgressBar())
            {
                foreach (var path in list)
                {
                    try
                    {
                        using (var stream = File.OpenRead(path))
                        {
                            var folder = Path.GetDirectoryName(path) ?? "";
                            var annotation = annotationLoader.ParseFromXml(stream);
                            var photoPath = Path.Combine(folder, annotation.Filename);
                            using (var photoStream = File.OpenRead(photoPath))
                            {
                                var (brush, height, width) = await reader.Read(photoStream);
                                var (metadata, latitude, longitude) = ExifConvertor.ConvertExif(
                                    ImageMetadataReader.ReadMetadata(photoPath));
                                var photoViewModel = new PhotoViewModel(index)
                                {
                                    Brush = brush,
                                    Detections = annotation.GetDetections(),
                                    Height = height,
                                    Width = width,
                                    Path = path,
                                    Latitude = latitude,
                                    Longitude = longitude,
                                    ExifDataCollection = metadata
                                };
                                photoList.Add(photoViewModel);
                            }
                            index++;
                            Notify?.Invoke(Enums.Status.Working, $"Working | {(int)((double) index / list.Length * 100)} %, [{index} of {list.Length}]");
                            pb.Report((double) index / list.Length, $"Processed {index} of {list.Length}");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e,$"image from {path} is skipped!");
                    }
                }
            }
            return photoList.ToArray();
        }

        public async Task<PhotoViewModel> LoadFromFile(string path, int index, IEnumerable<IObject> detections, LoadType loadType = LoadType.Miniature)
        {
            var reader = new AvaloniaBrushReader(loadType);
            using (var stream = File.OpenRead(path))
            {
                using (var photoStream = File.OpenRead(path))
                {
                    var (brush, height, width) = await reader.Read(photoStream);
                    var (metadata, latitude, longitude) = ExifConvertor.ConvertExif(
                        ImageMetadataReader.ReadMetadata(path));
                    return new PhotoViewModel(index)
                    {
                        Brush = brush,
                        Detections = detections,
                        Height = height,
                        Width = width,
                        Path = path,
                        Latitude = latitude,
                        Longitude = longitude,
                        ExifDataCollection = metadata
                    };
                }
            }
        }
    }
}