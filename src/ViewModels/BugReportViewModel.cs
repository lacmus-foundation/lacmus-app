using System;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using LacmusApp.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class BugReportViewModel : ReactiveObject
    {
        public BugReportViewModel(Window window, LocalizationContext localizationContext)
        {
            LocalizationContext = localizationContext;
            OpenFalseNegativeCommand = ReactiveCommand.Create(OpenFalseNegative);
            OpenFalsePositiveCommand  = ReactiveCommand.Create(OpenFalsePositive);
        }
        [Reactive] public LocalizationContext LocalizationContext { get; set; }
        public ReactiveCommand<Unit, Unit> OpenFalseNegativeCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenFalsePositiveCommand { get; set; }
        
        public void OpenFalseNegative()
        {
            OpenUrl("https://forms.gle/QbJaqcYvozC1dqTg7");
        }
        public void OpenFalsePositive()
        {
            OpenUrl("https://forms.gle/FAGyGsSiFooVt5L89");
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
    }
}