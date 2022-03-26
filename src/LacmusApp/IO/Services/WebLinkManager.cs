using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LacmusApp.IO.Interfaces;
using Serilog;

namespace LacmusApp.IO.Services
{
    public class WebLinkManager : IWebLinkManager
    {
        public void OpenLink(string link)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //https://stackoverflow.com/a/2796367/241446
                    using (Process proc = new Process {StartInfo = {UseShellExecute = true, FileName = link}})
                    {
                        proc.Start();
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("x-www-browser", link);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", link);
                }
                else
                    throw new Exception();
            }
            catch (Exception e)
            {
                Log.Error(e,$"Unable to ope url {link}.");
            }
        }
    }
}