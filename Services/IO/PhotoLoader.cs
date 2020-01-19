using System;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using MetadataExtractor;
using RescuerLaApp.Models.Photo;
using SkiaSharp;
using Attribute = RescuerLaApp.Models.Photo.Attribute;

namespace RescuerLaApp.Services.IO
{
    public class PhotoLoader : IPhotoLoader
    {
        public Photo Load(string source, PhotoLoadType loadType)
        {
            var loader = new FileLoader();
            using (var stream = loader.Load(source))
            {
                try
                {
                    var imageBrush = ReadImageBrushFromFile(stream, loadType);
                    var metaDataDirectories = ImageMetadataReader.ReadMetadata(stream);
                    var photo = new Photo
                    {
                        ImageBrush = imageBrush,
                        Attribute = Attribute.NotProcessed,
                        Caption = GetCaptionFromPath(source),
                        MetaDataDirectories = metaDataDirectories
                    };
                    return photo;
                }
                catch (Exception e)
                {
                    throw new Exception($"unable to load image from {source}", e);
                }
            }
        }
        
        private static string GetCaptionFromPath(string path)
        {
            var name = System.IO.Path.GetFileName(path);
            if (name.Length > 10)
            {
                name = name.Substring(0, 3) + "{~}" + name.Substring(name.Length - 5);
            }
            return name;
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
                            (int)(src.Width * scale),
                            (int)(src.Height * scale), 
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
                    throw new Exception($"invalid PhotoLoadType: {loadType.ToString()}");
            }
        }
    }
}