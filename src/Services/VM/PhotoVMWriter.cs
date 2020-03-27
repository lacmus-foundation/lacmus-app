using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using LacmusApp.Models.Photo;
using LacmusApp.Services.Files;
using LacmusApp.ViewModels;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LacmusApp.Models;
using LacmusApp.Services.IO;
using Serilog;
using ProgressBar = LacmusApp.Models.ProgressBar;

namespace LacmusApp.Services.VM
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
                throw new Exception("Unable to save photo.", e);
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
                var count = 0;
                var viewModels = photoViewModels as PhotoViewModel[] ?? photoViewModels.ToArray();
                using (var pb = new ProgressBar())
                {
                    foreach (var photoViewModel in viewModels)
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
                                Log.Warning($"Photo {srcPhotoPath} skipped. File exists.");
                                pb.Report((double)count / viewModels.Count(), $"Saving files {count} of {viewModels.Length}");
                                return;
                            }
                            File.Copy(srcPhotoPath, dstPhotoPath, true);
                        });
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable to save photo!",e);
            }
        }
    }
}