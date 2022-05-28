using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace LacmusApp.Avalonia.Services.Files
{
    public interface IAvaloniaFileSelector
    {
        Task<string> SelectFile(OpenFileDialog fileDialog);
        Task<IEnumerable<string>> SelectFiles(OpenFileDialog fileDialog);
        Task<IEnumerable<string>> SelectAllFilesFromDir(OpenFolderDialog folderDialog, bool isRecursive = false);
    }
}