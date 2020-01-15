using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace RescuerLaApp.Interfaces
{
    public interface IFileWriter
    {
        Task<Stream> Write(string name, OpenFolderDialog folderDialog);
    }
}