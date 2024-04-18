using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LacmusApp.Image.Interfaces;
using LacmusApp.Image.Models;
using LacmusPlugin;
using MetadataExtractor;
using Serilog;
using Directory = System.IO.Directory;

namespace LacmusApp.Image.Services
{
    public class ImageLoader<TBrush> : IImageLoader<TBrush>
    {
        private IBrushReader<TBrush> _brushReader;

        public ImageLoader(IBrushReader<TBrush> brushReader)
        {
            _brushReader = brushReader;
        }

        public async Task<IImage<TBrush>> LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException($"No such file {path}");

            await using (var stream = File.OpenRead(path))
            {
                var (metadata, latitude, longitude, altitude) = ExifConvertor.ConvertExif(
                    ImageMetadataReader.ReadMetadata(stream));
                var (imageBrush, width, height) = await _brushReader.Read(stream);

                return new Image<TBrush>()
                {
                    Detections = Array.Empty<IObject>(),
                    ExifDataCollection = metadata,
                    Brush = imageBrush,
                    Height = height,
                    Width = width,
                    IsFavorite = false,
                    IsWatched = false,
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = altitude,
                    Path = path
                };
            }
        }

        public async Task<IEnumerable<IImage<TBrush>>> LoadFromDirectory(string path, bool isRecursive = false)
        {
            if (!Directory.Exists(path))
                throw new InvalidOperationException($"No such directory {path}");

            var pathList = Directory.GetFiles(path, "*.*",
                    isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(s =>
                    s.ToLower().EndsWith(".png") ||
                    s.ToLower().EndsWith(".jpg") ||
                    s.ToLower().EndsWith(".jpeg"));
            var result = new List<IImage<TBrush>>();
            foreach (var p in pathList)
            {
                try
                {
                    result.Add(await LoadFromFile(p));
                }
                catch (Exception e)
                {
                    Log.Warning($"File skipped. {e.Message}");
                }
            }

            return result;
        }

        public async Task<IImage<TBrush>> LoadFromXml(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException($"No such file {path}");

            await using (var xmlStream = File.OpenRead(path))
            {
                var annotationLoader = new AnnotationLoader();
                var annotation = annotationLoader.ParseFromXml(xmlStream);
                var baseDir = Path.GetDirectoryName(path);
                if (baseDir == null)
                    throw new InvalidOperationException($"Base directory is null");

                var imagePath = Path.Combine(baseDir, annotation.Filename);
                var image = await LoadFromFile(imagePath);
                var objects = new List<IObject>();
                image.Detections = annotation.Objects.Select(x => new Detection()
                {
                    Label = x.Name,
                    Score = 1f,
                    XMax = x.Box.Xmax,
                    XMin = x.Box.Xmin,
                    YMax = x.Box.Ymax,
                    YMin = x.Box.Ymin
                });
                return image;
            }
        }

        public async Task<IEnumerable<IImage<TBrush>>> LoadFromDirectoryWithXml(string path, bool isRecursive = false)
        {
            if (!Directory.Exists(path))
                throw new InvalidOperationException($"No such directory {path}");

            var pathList = Directory.GetFiles(path, "*.*",
                    isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(s => s.ToLower().EndsWith(".xml"));
            var result = new List<IImage<TBrush>>();
            foreach (var p in pathList)
            {
                try
                {
                    result.Add(await LoadFromXml(p));
                }
                catch (Exception e)
                {
                    Log.Warning($"File skipped. {e.Message}");
                }
            }

            return result;
        }
    }
}