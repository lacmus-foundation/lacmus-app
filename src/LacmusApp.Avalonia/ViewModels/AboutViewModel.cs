using System;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using LacmusApp.Avalonia.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace LacmusApp.Avalonia.ViewModels
{
    public class AboutViewModel : ReactiveObject
    {
        public AboutViewModel(Window window, LocalizationContext localizationContext)
        {
            LocalizationContext = localizationContext;
            OpenLicenseCommand = ReactiveCommand.Create(OpenLicense);
            OpenGithubCommand = ReactiveCommand.Create(OpenGithub);
            OpenSiteCommand = ReactiveCommand.Create(OpenSite);
        }
        [Reactive] public LocalizationContext LocalizationContext { get; set; }
        [Reactive] public string TextVersion { get; set; } = GetVersion() + ".";
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
                    //https://stackoverflow.com/a/2796367/241446
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
        
        private static string GetVersion()
        {
            var revision = "";
            if (typeof(Program).Assembly.GetName().Version.Revision != 0)
                revision = $"preview-{typeof(Program).Assembly.GetName().Version.Revision}";
            return $"{typeof(Program).Assembly.GetName().Version.Major}.{typeof(Program).Assembly.GetName().Version.Minor}.{typeof(Program).Assembly.GetName().Version.Build}.{revision} beta";
        }
    }
}