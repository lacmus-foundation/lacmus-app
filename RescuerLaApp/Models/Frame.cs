using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;

namespace RescuerLaApp.Models
{
    public class Frame
    {
        public string Path { get; set; }

        public string Name
        {
            get
            {
                var name = System.IO.Path.GetFileName(Path);
                if (name.Length > 10)
                {
                    name = name.Substring(0, 3) + "{~}" + name.Substring(name.Length - 5);
                }
                return name;
            }
        }

        public ImageBrush ImageBrush { get; set; } = new ImageBrush { Stretch = Stretch.Uniform };

        public Bitmap Bitmap { get; set; }

        public IEnumerable<BoundBox> Rectangles { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool IsVisible { get; set; } = false;
        public bool IsFavorite { get; set; } = false;
        
        public delegate void MethodContainer();

        public event MethodContainer OnLoad;

        public void Load(string imgFileName, Enums.ImageLoadMode loadMode = Enums.ImageLoadMode.Full)
        {
            
            Task.Factory.StartNew(() =>
            {
                Path = imgFileName;
                switch (loadMode)
                {
                    case Enums.ImageLoadMode.Full:
                        Bitmap = new Bitmap(imgFileName);
                        Width = Bitmap.PixelSize.Width;
                        Height = Bitmap.PixelSize.Height;
                        break;
                    case Enums.ImageLoadMode.Miniature:
                        using (var stream = new SKFileStream(imgFileName))
                        using (var src = SKBitmap.Decode(stream))
                        {
                            Width = src.Width;
                            Height = src.Height;
                            var scale = 100f / src.Width;
                            var resized = new SKBitmap(
                                (int)(src.Width * scale),
                                (int)(src.Height * scale), 
                                src.ColorType, 
                                src.AlphaType);
                            src.ScalePixels(resized, SKFilterQuality.Low);
                            Bitmap = new Bitmap(
                                resized.ColorType.ToPixelFormat(),
                                resized.GetPixels(),
                                new PixelSize(resized.Width, resized.Height), 
                                SkiaPlatform.DefaultDpi, 
                                resized.RowBytes);
                        }
                        break;
                    default:
                        throw new Exception($"invalid ImageLoadMode:{loadMode.ToString()}");
                }
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ImageBrush.Source = Bitmap;
                    OnLoad?.Invoke();
                });
            });
        }
    }
}