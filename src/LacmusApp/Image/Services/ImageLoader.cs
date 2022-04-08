using System;
using System.Collections.Generic;
using System.IO;
using LacmusApp.Image.Interfaces;
using LacmusApp.Image.Models;
using LacmusPlugin;
using MetadataExtractor;

namespace LacmusApp.Image.Services
{
    public class ImageLoader<TBrush> : IImageLoader<TBrush>
    {
        private IBrushReader<TBrush> _brushReader;
        
        public ImageLoader(IBrushReader<TBrush> brushReader)
        {
            _brushReader = brushReader;
        }
        
        public IImage<TBrush> LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException($"No such file {path}");

            using (var stream = File.OpenRead(path))
            {
                var (metadata, latitude, longitude) = ExifConvertor.ConvertExif(
                        ImageMetadataReader.ReadMetadata(stream));
                var (imageBrush, width, height) = _brushReader.Read(stream);

                return new Image<TBrush>()
                {
                    Detections = Array.Empty<IObject>(),
                    ExifDataCollection = metadata,
                    Brush = imageBrush,
                    Height = height,
                    Width = width,
                    IsFavorite = false,
                    IsHasObjects = false,
                    IsWatched = false,
                    Latitude = latitude,
                    Longitude = longitude,
                    Path = path
                };
            }
        }

        public IEnumerable<IImage<TBrush>> LoadFromDirectory(string path)
        {
            throw new System.NotImplementedException();
        }

        public IImage<TBrush> LoadFromXml(string path)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IImage<TBrush>> LoadFromDirectoryWithXml(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}