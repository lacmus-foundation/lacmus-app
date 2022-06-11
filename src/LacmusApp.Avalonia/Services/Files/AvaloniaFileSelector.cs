using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace LacmusApp.Avalonia.Services.Files
{
    public class AvaloniaFileSelector : IAvaloniaFileSelector
    {
        private readonly Window _window;

        public AvaloniaFileSelector(Window window) => _window = window;

        public async Task<string> SelectFile(OpenFileDialog fileDialog = null)
        {
            fileDialog ??= new OpenFileDialog();
            fileDialog.AllowMultiple = false;
            var files = await fileDialog.ShowAsync(_window);
            var path = files.First();
            
            var attributes = File.GetAttributes(path);
            var isFolder = attributes.HasFlag(FileAttributes.Directory);
            if (isFolder) throw new Exception("Folders are not supported.");
            return path;
        }

        public async Task<string> SelectDir(OpenFolderDialog folderDialog)
        {
            folderDialog ??= new OpenFolderDialog();
            var path = await folderDialog.ShowAsync(_window);

            var attributes = File.GetAttributes(path);
            var isFolder = attributes.HasFlag(FileAttributes.Directory);
            if (!isFolder) throw new Exception("Files are not supported.");
            return path;
        }

        public async Task<IEnumerable<string>> SelectFiles(OpenFileDialog fileDialog = null)
        {
            fileDialog ??= new OpenFileDialog();
            fileDialog.AllowMultiple = true;
            var files = await fileDialog.ShowAsync(_window);
            return files.Where(x => File.GetAttributes(x).HasFlag(FileAttributes.Directory));
        }

        public async Task<IEnumerable<string>> SelectAllFilesFromDir(OpenFolderDialog fileDialog = null, bool isRecursive = false)
        {
            fileDialog ??= new OpenFolderDialog();

            var dirPath = await fileDialog.ShowAsync(_window);
            
            if (!Directory.Exists(dirPath))
                return Enumerable.Empty<string>();
            
            var files = GetFilesFromDir(dirPath, isRecursive);
            return files;
        }
        
        private static IEnumerable<string> GetFilesFromDir(string dirPath, bool isRecursive)
        {
            return Directory.GetFiles(dirPath, "*.*",
                isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }
    }
}