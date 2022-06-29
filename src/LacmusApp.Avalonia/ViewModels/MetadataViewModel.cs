using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using DynamicData;
using LacmusApp.Avalonia.Services;
using LacmusApp.Image.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using SkiaSharp;
using SkiaSharp.QrCode;


namespace LacmusApp.Avalonia.ViewModels
{
    public class MetadataViewModel : ReactiveValidationObject
    {
        private SourceList<ExifData> _metaDataList { get; set; } = new SourceList<ExifData>();
        private ReadOnlyObservableCollection<ExifData> _metaDataCollection;
        public ReadOnlyObservableCollection<ExifData> MetaDataCollection => _metaDataCollection;
        [Reactive] public string Latitude { get; set; } = "N/A";
        [Reactive] public string Longitude { get; set; } = "N/A";
        [Reactive] public ImageBrush QrImage { get; set; }
        [Reactive] public LocalizationContext LocalizationContext { get; set; }
        public MetadataViewModel(Window window, PhotoViewModel photoViewModel, LocalizationContext localizationContext)
        {
            Latitude = $"{photoViewModel.Latitude}";
            Longitude = $"{photoViewModel.Longitude}";
            
            _metaDataList.AddRange(photoViewModel.ExifDataCollection);

            LocalizationContext = localizationContext;
            
            _metaDataList
                .Connect()
                .Bind(out _metaDataCollection)
                .Subscribe();
            
            this.ValidationRule(
                viewModel => viewModel.Latitude,
                x => x != "N/A",
                path => $"Cannot parse gps latitude");
            this.ValidationRule(
                viewModel => viewModel.Longitude,
                x => x != "N/A",
                path => $"Cannot parse gps longitude");

            OpenYandexCommand = ReactiveCommand.Create(OpenYandex, this.IsValid());
            OpenGoogleCommand = ReactiveCommand.Create(OpenGoogle, this.IsValid());
            OpenOSMCommand = ReactiveCommand.Create(OpenOSM, this.IsValid());

            RenderQr();
        }
        public ReactiveCommand<Unit, Unit> OpenYandexCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenGoogleCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenOSMCommand { get; set; }
        
        public void OpenYandex()
        {
            OpenUrl($"https://yandex.ru/maps/?ll={Longitude.Replace(',', '.')}%2C{Latitude.Replace(',', '.')}&z=15"+
                    $"&mode=whatshere&whatshere%5Bpoint%5D={Longitude.Replace(',', '.')}%2C{Latitude.Replace(',', '.')}&whatshere%5Bzoom%5D=15");
        }
        public void OpenGoogle()
        {
            //https://www.google.com/maps/place/"&[Latitude]&"+"&[Longitude]&"/@"&[Latitude]&","&[Longitude]&",15z
            OpenUrl($"https://www.google.ru/maps/place/{Latitude.Replace(',', '.')}+{Longitude.Replace(',', '.')}"+
                    $"/@{Latitude.Replace(',', '.')},{Longitude.Replace(',', '.')},15z");
        }
        
        public void OpenOSM()
        {
            OpenUrl($"https://www.openstreetmap.org/?mlat={Latitude.Replace(',', '.')}&mlon={Longitude.Replace(',', '.')}"+
                    $"#map=15/{Latitude.Replace(',', '.')}/{Longitude.Replace(',', '.')}");
        }
        
        private string TranslateGeoTag(string tag)
        {
            try
            {
                if (!tag.Contains('°'))
                    return tag;
                tag = tag.Replace('°', ';');
                tag = tag.Replace('\'', ';');
                tag = tag.Replace('"', ';');
                tag = tag.Replace(" ", "");

                var splitTag = tag.Split(';');
                var grad = float.Parse(splitTag[0]);
                var min = float.Parse(splitTag[1]);
                var sec = float.Parse(splitTag[2]);

                var result = grad + min / 60 + sec / 3600;
                return $"{result}";
            }
            catch
            {
                return "N/A";
            }
        }
        private void OpenUrl(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using (Process proc = new Process {StartInfo = {UseShellExecute = true, FileName = url}})
                    {
                        proc.Start();
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("x-www-browser", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                    throw new Exception();
            }
            catch (Exception e)
            {
                Log.Error(e,$"Unable to ope url {url}.");
            }
        }

        private void RenderQr()
        {
            using (var generator = new QRCodeGenerator())
            {
                var osmLink =
                    $"https://www.openstreetmap.org/?mlat={Latitude.Replace(',', '.')}&mlon={Longitude.Replace(',', '.')}" +
                    $"#map=15/{Latitude.Replace(',', '.')}/{Longitude.Replace(',', '.')}";
                var qr = generator.CreateQrCode(osmLink, ECCLevel.L);
                var info = new SKImageInfo(128, 128);
                using (var surface = SKSurface.Create(info))
                {
                    surface.Canvas.Render(qr, info.Width, info.Height);
                    var image = surface.Snapshot();
                    var skBitmap = SKBitmap.FromImage(image);
                    var bitmap = new Bitmap(
                        skBitmap.ColorType.ToPixelFormat(),
                        skBitmap.GetPixels(),
                        new PixelSize(skBitmap.Width, skBitmap.Height), 
                        SkiaPlatform.DefaultDpi, 
                        skBitmap.RowBytes);
                    QrImage = new ImageBrush(bitmap);
                }
            }
        }
    }
}