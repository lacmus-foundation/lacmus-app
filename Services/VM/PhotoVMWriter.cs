using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using RescuerLaApp.Models.Photo;
using RescuerLaApp.Services.Files;
using RescuerLaApp.ViewModels;
using System.IO;
using System.Xml.Serialization;
using RescuerLaApp.Models;
using RescuerLaApp.Services.IO;

namespace RescuerLaApp.Services.VM
{
    public class PhotoVMWriter
    {
        private readonly Window _window;

        public PhotoVMWriter(Window window)  => _window = window;

        public async Task Write(PhotoViewModel photoViewModel)
        {
            var folderDialog = new OpenFolderDialog
            {
                //TODO: Multi language support
                Title = "Select folder to save"
            };
            try
            {
                await Task.Run(async () =>
                    {
                        var folder = await folderDialog.ShowAsync(_window);
                        var srcPhotoPath = photoViewModel.Path;
                        var dstPhotoPath = Path.Combine(folder, photoViewModel.Annotation.Filename);
                        var annotationPath = Path.Combine(folder, $"{photoViewModel.Annotation.Filename}.xml");
                        var annotation = photoViewModel.Annotation;
                        annotation.Folder = folder;
                        var saver = new AnnotationSaver();
                        saver.Save(annotation, annotationPath);
                        File.Copy(srcPhotoPath, dstPhotoPath, true);
                    });
            }
            catch (Exception e)
            {
                //TODO: translate to rus
                Console.WriteLine($"ERROR: con not save photo!\nDetails: {e}");
            }
        }

        public async Task WriteMany(IEnumerable<PhotoViewModel> photoViewModels)
        {
            var folderDialog = new OpenFolderDialog
            {
                //TODO: Multi language support
                Title = "Select folder to save"
            };
            try
            {
                var folder = await folderDialog.ShowAsync(_window);
                if(!Directory.Exists(folder))
                    return;
                foreach (var photoViewModel in photoViewModels)
                    await Task.Run(() =>
                    {
                        var srcPhotoPath = photoViewModel.Path;
                        var dstPhotoPath = Path.Combine(folder, photoViewModel.Annotation.Filename);
                        var annotationPath = Path.Combine(folder, $"{photoViewModel.Annotation.Filename}.xml");
                        var annotation = photoViewModel.Annotation;
                        annotation.Folder = folder;
                        var saver = new AnnotationSaver();
                        saver.Save(annotation, annotationPath);
                        if (srcPhotoPath == dstPhotoPath)
                        {
                            Console.WriteLine($"WARN: photo {srcPhotoPath} skipped. File exists.");
                            return;
                        }
                        File.Copy(srcPhotoPath, dstPhotoPath, true);
                    });
            }
            catch (Exception e)
            {
                throw new Exception("Unable to save photo!",e);
            }
        }
    }
}