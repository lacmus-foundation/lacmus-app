using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LacmusPlugin;

namespace LacmusApp.Plugin.Interfaces
{
    public interface IWebClient : IDisposable
    {
        Task<int> GetMaxPage();
        Task<Stream> GetZipFile(string tag, int api, int major, int minor);
        Task<IEnumerable<IObjectDetectionPlugin>> GetPluginInfoFromPage(int page);
    }
}