using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using RescuerLaApp.Interfaces;

namespace RescuerLaApp.Services
{
    public class AvaloniaImageFileWriter : IFileWriter
    {
        public Task<Stream> Write(string name, OpenFolderDialog folderDialog)
        {
            throw new System.NotImplementedException();
        }
    }
}