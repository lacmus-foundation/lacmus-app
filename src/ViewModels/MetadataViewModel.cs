using System;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using ReactiveUI;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class MetadataViewModel : ReactiveObject
    {
        public MetadataViewModel(Window window)
        {
            OpenYandexCommand = ReactiveCommand.Create(OpenYandex);
            OpenGoogleCommand = ReactiveCommand.Create(OpenGoogle);
        }
        public ReactiveCommand<Unit, Unit> OpenYandexCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenGoogleCommand { get; set; }
        
        public void OpenYandex()
        {
            var lat = "54.719981";
            var lng = "20.534811";
            OpenUrl($"https://yandex.ru/maps/?ll={lng}%2C{lat}&z=15");
        }
        public void OpenGoogle()
        {
            var lat = "54.719981";
            var lng = "20.534811";
            OpenUrl($"https://www.google.ru/maps/@{lat},{lng},15z");
        }
        
        private void OpenUrl(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(url);
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
    }
}