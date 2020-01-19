using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace RescuerLaApp.Services.Files
{
    public interface IFileReader
    {
        Task<(string Path, Stream Stream)> Read(OpenFileDialog fileDialog);
        Task<(string Path, Stream Stream)[]> ReadMultiple(OpenFileDialog fileDialog);
        Task<(string Path, Stream Stream)[]> ReadAllFromDir(OpenFileDialog fileDialog, bool isRecursive = false);
    }
}