using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace LacmusApp.Avalonia.Services.Files
{
    public interface IFileWriter
    {
        Task<Stream> Write(string name, OpenFolderDialog folderDialog);
    }
}