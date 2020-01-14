using System.IO;
using System.Threading.Tasks;
using RescuerLaApp.Interfaces;

namespace RescuerLaApp.Services
{
    public class AvaloniaImageFileWriter : IFileWriter
    {
        public Task<Stream> Write(string name)
        {
            throw new System.NotImplementedException();
        }
    }
}