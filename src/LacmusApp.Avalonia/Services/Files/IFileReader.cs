using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace LacmusApp.Avalonia.Services.Files
{
    public interface IFileReader
    {
        Task<(string Path, Stream Stream)> Read(OpenFileDialog fileDialog);
        Task<IEnumerable<(string Path, Stream Stream)>> ReadMultiple(OpenFileDialog fileDialog);
        Task<IEnumerable<(string Path, Stream Stream)>> ReadAllFromDir(OpenFolderDialog folderDialog, bool isRecursive = false);
    }
}