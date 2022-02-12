using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace LacmusApp.Avalonia.Services.Files
{
    public class AvaloniaPhotoFileWriter : IFileWriter
    {
        public Task<Stream> Write(string name, OpenFolderDialog folderDialog)
        {
            throw new System.NotImplementedException();
        }
    }
}