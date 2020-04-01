using System;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using ReactiveUI;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class AboutViewModel : ReactiveObject
    {
        public AboutViewModel(Window window)
        {
            OpenLicenseCommand = ReactiveCommand.Create(OpenLicense);
            OpenGithubCommand = ReactiveCommand.Create(OpenGithub);
            OpenSiteCommand = ReactiveCommand.Create(OpenSite);
        }
        public ReactiveCommand<Unit, Unit> OpenLicenseCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenGithubCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenSiteCommand { get; set; }
        
        public void OpenLicense()
        {
            OpenUrl("https://github.com/lacmus-foundation/lacmus-app/blob/master/LICENSE");
        }
        public void OpenGithub()
        {
            OpenUrl("https://github.com/lacmus-foundation/");
        }
        public void OpenSite()
        {
            OpenUrl("https://lacmus-foundation.github.io/");
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