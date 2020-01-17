using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace RescuerLaApp.Services.Files
{
    public class AvaloniaFileReader : IFileReader
    {
        private readonly Window _window;

        public AvaloniaFileReader(Window window) => _window = window;

        public async Task<(string Name, Stream Stream)> Read(OpenFileDialog fileDialog = null)
        {
            if (fileDialog == null)
                fileDialog = new OpenFileDialog();
            fileDialog.AllowMultiple = false;
            var files = await fileDialog.ShowAsync(_window);
            var path = files.First();
            
            var attributes = File.GetAttributes(path);
            var isFolder = attributes.HasFlag(FileAttributes.Directory);
            if (isFolder) throw new Exception("Folders are not supported.");

            var stream = File.OpenRead(path);
            var name = Path.GetFileName(path);
            return (name, stream);
        }

        public async Task<(string Name, Stream Stream)[]> ReadMultiple(OpenFileDialog fileDialog = null)
        {
            if (fileDialog == null)
                fileDialog = new OpenFileDialog();
            fileDialog.AllowMultiple = true;
            var files = await fileDialog.ShowAsync(_window);
            var result = new List<(string Name, Stream Stream)>();
            foreach (var file in files)
            {
                var attributes = File.GetAttributes(file);
                var isFolder = attributes.HasFlag(FileAttributes.Directory);
                if (isFolder) throw new Exception("Folders are not supported.");
                var stream = File.OpenRead(file);
                var name = Path.GetFileName(file);
                result.Add((name, stream));
            }
            return  result.ToArray();
        }

        public async Task<(string Name, Stream Stream)[]> ReadAllFromDir(OpenFileDialog fileDialog = null, bool isRecursive = false)
        {
            if (fileDialog == null)
                fileDialog = new OpenFileDialog();
            fileDialog.AllowMultiple = false;
            var dirPaths = await fileDialog.ShowAsync(_window);
            var dirPath = dirPaths.First();
            
            var attributes = File.GetAttributes(dirPath);
            var isFolder = attributes.HasFlag(FileAttributes.Directory);
            if (!isFolder) throw new Exception("Files are not supported.");
            
            var files = GetFilesFromDir(dirPath, isRecursive);
            var result = new List<(string Name, Stream Stream)>();
            foreach (var file in files)
            {
                var stream = File.OpenRead(file);
                var name = Path.GetFileName(file);
                result.Add((name, stream));
            }
            return  result.ToArray();
        }

        //TODO: Create Recursive Search
        private static IEnumerable<string> GetFilesFromDir(string dirPath, bool isRecursive)
        {
            return Directory.GetFiles(dirPath);
        }
    }
}