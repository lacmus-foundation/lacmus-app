using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace LacmusApp.Avalonia.Services.Files
{
    public class AvaloniaFileReader : IFileReader
    {
        private readonly Window _window;

        public AvaloniaFileReader(Window window) => _window = window;

        public async Task<(string Path, Stream Stream)> Read(OpenFileDialog fileDialog = null)
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
            return (path, stream);
        }

        public async Task<IEnumerable<(string Path, Stream Stream)>> ReadMultiple(OpenFileDialog fileDialog = null)
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
                result.Add((file, stream));
            }
            return  result.ToArray();
        }

        public async Task<IEnumerable<(string Path, Stream Stream)>> ReadAllFromDir(OpenFolderDialog fileDialog = null, bool isRecursive = false)
        {
            if (fileDialog == null)
                fileDialog = new OpenFolderDialog();

            var dirPath = await fileDialog.ShowAsync(_window);
            
            if (!Directory.Exists(dirPath))
            {
                //NOTE possible exception raise here instead of  empty return
                return Enumerable.Empty<(string Path, Stream Stream)>();
            }
           
            var files = GetFilesFromDir(dirPath, isRecursive);
            var result = new List<(string Name, Stream Stream)>();
            foreach (var file in files)
            {
                var stream = File.OpenRead(file);
                result.Add((file, stream));
            }
            return  result;
        }

        //TODO: Create Recursive Search
        private static IEnumerable<string> GetFilesFromDir(string dirPath, bool isRecursive)
        {
            return Directory.GetFiles(dirPath, "*.*",//NOTE probably need to make  more specialized -- *.png, *.jpeg...
                isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
    }
}