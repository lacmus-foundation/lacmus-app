using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using LacmusApp.IO.Interfaces;

namespace LacmusApp.Avalonia.Services
{
    public class AvaloniaPluginFileManager : IFileManager
    {
        private Window _window;
        
        public AvaloniaPluginFileManager(Window window)
        {
            _window = window;
        }
        
        public async Task<string> SelectFileToWrite()
        {
            var dig = new SaveFileDialog()
            {
                Filters = new List<FileDialogFilter>()
                {
                    new()
                    {
                        Extensions = new() {"zip", "ZIP"}
                    }
                },
                Title = "Chose plugin"
            };
            
            var file = await dig.ShowAsync(_window);
            
            if (file == null)
                throw new Exception("File is not selected");
            return file;
        }

        public async Task<string> SelectFileToRead()
        {
            var dig = new OpenFileDialog()
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>()
                {
                    new()
                    {
                        Extensions = new() {"zip", "ZIP"}
                    }
                },
                Title = "Chose plugin"
            };
            
            var files = await dig.ShowAsync(_window);
            
            if (files == null)
                throw new Exception("File is not selected");
            var file = files.First();
            if (!File.Exists(file))
                throw new Exception("File is not exists");
            var attributes = File.GetAttributes(file);
            if (attributes.HasFlag(FileAttributes.Directory))
                throw new Exception("Folders are not supported");
            
            return file;
        }
    }
}