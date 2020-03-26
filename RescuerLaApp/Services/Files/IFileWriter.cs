using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace RescuerLaApp.Services.Files
{
    public interface IFileWriter
    {
        Task<Stream> Write(string name, OpenFolderDialog folderDialog);
    }
}