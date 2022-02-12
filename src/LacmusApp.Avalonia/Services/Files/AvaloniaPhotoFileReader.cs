using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using MetadataExtractor;
using LacmusApp.Avalonia.Models.Photo;
using SkiaSharp;
using Attribute = LacmusApp.Avalonia.Models.Photo.Attribute;

namespace LacmusApp.Avalonia.Services.Files
{
    public class AvaloniaPhotoFileReader : IPhotoReader
    {
        private readonly AvaloniaFileReader _reader;

        public AvaloniaPhotoFileReader(Window window)  => _reader = new AvaloniaFileReader(window);
        public async Task<Photo> Read(PhotoLoadType loadType = PhotoLoadType.Miniature)
        {
            var dig = new OpenFileDialog()
            {
                //TODO: Multi language support
                Title = "Chose the image file"
            };
            var (path, stream) = await _reader.Read(dig);
            try
            {
                var imageBrush = ReadImageBrushFromFile(stream, loadType);
                var metaDataDirectories = ImageMetadataReader.ReadMetadata(stream);
                var photo = new Photo
                {
                    ImageBrush = imageBrush,
                    Attribute = Attribute.NotProcessed,
                    MetaDataDirectories = metaDataDirectories
                };
                return photo;
            }
            catch (Exception e)
            {
                throw new Exception($"unable to read image from {path}");
            }
        }

        public async Task<Photo[]> ReadMultiple(PhotoLoadType loadType = PhotoLoadType.Miniature)
        {
            var dig = new OpenFileDialog()
            {
                //TODO: Multi language support
                Title = "Chose image files"
            };
            var multipleFiles = await _reader.ReadMultiple(dig);
            var photoList = new List<Photo>();
            foreach (var (path,stream) in multipleFiles)
            {
                try
                {
                    var imageBrush = ReadImageBrushFromFile(stream, loadType);
                    var metaDataDirectories = ImageMetadataReader.ReadMetadata(stream);
                    var photo = new Photo
                    {
                        ImageBrush = imageBrush,
                        Attribute = Attribute.NotProcessed,
                        MetaDataDirectories = metaDataDirectories
                    };
                    photoList.Add(photo);
                }
                catch (Exception e)
                {
                    throw new Exception($"unable to read image from {path}");
                }
            }
            return photoList.ToArray();
        }

        public async Task<Photo[]> ReadAllFromDir(PhotoLoadType loadType = PhotoLoadType.Miniature, bool isRecursive = false)
        {
            var dig = new OpenFolderDialog()
            {
                //TODO: Multi language support
                Title = "Chose directory image files"
            };
            var multipleFiles = await _reader.ReadAllFromDir(dig, isRecursive);
            var photoList = new List<Photo>();
            foreach (var (path,stream) in multipleFiles)
            {
                try
                {
                    var imageBrush = ReadImageBrushFromFile(stream, loadType);
                    var metaDataDirectories = ImageMetadataReader.ReadMetadata(path);
                    var photo = new Photo
                    {
                        ImageBrush = imageBrush,
                        Attribute = Attribute.NotProcessed,
                        MetaDataDirectories = metaDataDirectories
                    };
                    photoList.Add(photo);
                }
                catch (Exception e)
                {
                    //TODO: translate to rus
                    Console.WriteLine($"ERROR: image from {path} is skipped!\nDetails: {e}");
                }
            }
            return photoList.ToArray();
        }

        private static ImageBrush ReadImageBrushFromFile(Stream stream, PhotoLoadType loadType)
        {
            switch (loadType)
            {
                case PhotoLoadType.Miniature:
                    using (var src = SKBitmap.Decode(stream))
                    {
                        var scale = 100f / src.Width;
                        var resized = new SKBitmap(
                            (int) (src.Width * scale),
                            (int) (src.Height * scale),
                            src.ColorType,
                            src.AlphaType);
                        src.ScalePixels(resized, SKFilterQuality.Low);
                        var bitmap = new Bitmap(
                            resized.ColorType.ToPixelFormat(),
                            resized.GetPixels(),
                            new PixelSize(resized.Width, resized.Height),
                            SkiaPlatform.DefaultDpi,
                            resized.RowBytes);
                        return new ImageBrush(bitmap);
                    }

                    break;
                case PhotoLoadType.Full:
                    return new ImageBrush(new Bitmap(stream));
                    break;
                default:
                    throw new Exception($"invalid ImageLoadMode:{loadType.ToString()}");
            }
        }
    }
}