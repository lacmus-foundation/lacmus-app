using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using RescuerLaApp.Interfaces;

namespace RescuerLaApp.Services
{
    public class AvaloniaImageFileReader : IFileReader
    {
        public Task<(string Name, Stream Stream)> Read()
        {
            throw new System.NotImplementedException();
        }

        public Task<(string Name, Stream Stream)[]> ReadMultiple()
        {
            throw new System.NotImplementedException();
        }

        public Task<(string Name, Stream Stream)> Read(OpenFileDialog fileDialog)
        {
            throw new System.NotImplementedException();
        }

        public Task<(string Name, Stream Stream)[]> ReadMultiple(OpenFileDialog fileDialog)
        {
            throw new System.NotImplementedException();
        }

        public Task<(string Name, Stream Stream)[]> ReadAllFromDir(OpenFileDialog fileDialog, bool isRecursive = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<(string Name, Stream Stream)[]> ReadAllFromDir(bool isRecursive = false)
        {
            throw new System.NotImplementedException();
        }
    }
}