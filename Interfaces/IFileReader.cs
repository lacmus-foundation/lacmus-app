using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace RescuerLaApp.Interfaces
{
    public interface IFileReader
    {
        Task<(string Name, Stream Stream)> Read(OpenFileDialog fileDialog);
        Task<(string Name, Stream Stream)[]> ReadMultiple(OpenFileDialog fileDialog);
        Task<(string Name, Stream Stream)[]> ReadAllFromDir(OpenFileDialog fileDialog, bool isRecursive = false);
    }
}