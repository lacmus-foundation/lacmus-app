using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using Avalonia.Threading;
using MetadataExtractor;
using LacmusApp.Avalonia.Models.Photo;
using SkiaSharp;
using Attribute = LacmusApp.Avalonia.Models.Photo.Attribute;

namespace LacmusApp.Avalonia.Services.IO
{
    public class PhotoLoader : IPhotoLoader
    {
        public async Task<Photo> Load(string source, PhotoLoadType loadType)
        {
            using (var stream = File.OpenRead(source))
            {
                try
                {
                    var imageBrush = await ReadImageBrushFromFile(stream, loadType);
                    var metaDataDirectories = ImageMetadataReader.ReadMetadata(source);
                    var photo = new Photo
                    {
                        ImageBrush = imageBrush.ImageBrush,
                        Attribute = Attribute.NotProcessed,
                        MetaDataDirectories = metaDataDirectories,
                        Height = imageBrush.Height,
                        Width = imageBrush.Width
                    };
                    return photo;
                }
                catch (Exception e)
                {
                    throw new Exception($"unable to load image from {source}", e);
                }
            }
        }
        public async Task<Photo> Load(string source, Stream stream, PhotoLoadType loadType)
        {
            using (stream)
            {
                try
                {
                    var imageBrush = await Task<LacmusImageBrush>.Factory.StartNew( () =>  ReadImageBrushFromFile(stream, loadType).Result);
                    var metaDataDirectories = ImageMetadataReader.ReadMetadata(source);
                    var photo = new Photo
                    {
                        ImageBrush = imageBrush.ImageBrush,
                        Attribute = Attribute.NotProcessed,
                        MetaDataDirectories = metaDataDirectories,
                        Height = imageBrush.Height,
                        Width = imageBrush.Width
                    };
                    return photo;
                }
                catch (Exception e)
                {
                    throw new Exception($"unable to load image from {source}", e);
                }
            }
        }
        
        private async Task<LacmusImageBrush> ReadImageBrushFromFile(Stream stream, PhotoLoadType loadType)
        {
            switch (loadType)
            {
                case PhotoLoadType.Miniature:
                    using (var src = SKBitmap.Decode(stream))
                    {
                        var width = src.Width;
                        var height = src.Height;
                        var scale = 100f / width;
                        var resized = new SKBitmap(
                            (int)(width * scale),
                            (int)(height * scale), 
                            src.ColorType, 
                            src.AlphaType);
                        src.ScalePixels(resized, SKFilterQuality.Low);
                        var bitmap = new Bitmap(
                            resized.ColorType.ToPixelFormat(),
                            resized.GetPixels(),
                            new PixelSize(resized.Width, resized.Height), 
                            SkiaPlatform.DefaultDpi, 
                            resized.RowBytes);
                        var imageBrush = await Dispatcher.UIThread.InvokeAsync(() => new ImageBrush(bitmap));
                        return new LacmusImageBrush{ Height = height, Width = width, ImageBrush = imageBrush };
                    }
                    break;
                case PhotoLoadType.Full:
                    var imageBrushFull = await Dispatcher.UIThread.InvokeAsync(() => new ImageBrush(new Bitmap(stream)));
                    return new LacmusImageBrush
                    {
                        Height = imageBrushFull.Source.PixelSize.Height,
                        Width = imageBrushFull.Source.PixelSize.Width,
                        ImageBrush = imageBrushFull
                    };
                    break;
                default:
                    throw new Exception($"invalid PhotoLoadType: {loadType.ToString()}");
            }
        }
    }
}