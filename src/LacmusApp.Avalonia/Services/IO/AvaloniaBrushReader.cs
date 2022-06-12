using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using Avalonia.Threading;
using LacmusApp.Image.Enums;
using LacmusApp.Image.Interfaces;
using SkiaSharp;

namespace LacmusApp.Avalonia.Services.IO
{
    public class AvaloniaBrushReader : IBrushReader<ImageBrush>
    {
        private LoadType _loadType;
        public AvaloniaBrushReader(LoadType loadType) => _loadType = loadType;
        public async Task<(ImageBrush, int, int)> Read(Stream stream)
        {
            switch (_loadType)
            {
                case LoadType.Miniature:
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
                        return (imageBrush, height, width);
                    }
                case LoadType.Full:
                    var brush = await Dispatcher.UIThread.InvokeAsync(() => new ImageBrush(new Bitmap(stream)));
                    return (brush, brush.Source.PixelSize.Height, brush.Source.PixelSize.Width);
                default:
                    throw new Exception($"Invalid LoadType {_loadType.ToString()}");
            }
        }
    }
}