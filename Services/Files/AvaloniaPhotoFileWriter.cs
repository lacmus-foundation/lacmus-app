using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using RescuerLaApp.Interfaces;

namespace RescuerLaApp.Services.Files
{
    public class AvaloniaPhotoFileWriter : IFileWriter
    {
        public Task<Stream> Write(string name, OpenFolderDialog folderDialog)
        {
            throw new System.NotImplementedException();
        }
    }
}