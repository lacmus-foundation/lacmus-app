using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using MetadataExtractor;
using RescuerLaApp.Models;
using RescuerLaApp.Models.Photo;
using RescuerLaApp.Services.Files;
using RescuerLaApp.Services.IO;
using RescuerLaApp.ViewModels;
using Attribute = RescuerLaApp.Models.Photo.Attribute;

namespace RescuerLaApp.Services.VM
{
    public class PhotoVMReader
    {
        private readonly AvaloniaFileReader _reader;
        
        public PhotoVMReader(Window window)  => _reader = new AvaloniaFileReader(window);
        
        public async Task<PhotoViewModel> ReadByPhoto(PhotoLoadType loadType = PhotoLoadType.Miniature)
        {
            var dig = new OpenFileDialog()
            {
                //TODO: Multi language support
                Title = "Chose the image file"
            };
            var photoLoader = new PhotoLoader();
            var (path, stream) = await _reader.Read(dig);
            try
            {
                var photo = photoLoader.Load(path, stream, loadType);
                var annotation = new Annotation
                {
                    Filename = Path.GetFileName(path),
                    Folder = Path.GetDirectoryName(path)
                };
                return new PhotoViewModel(photo, annotation);
            }
            catch (Exception e)
            {
                throw new Exception($"unable to read image from {path}");
            }
        }
        
        public async Task<PhotoViewModel> ReadByAnnotation(PhotoLoadType loadType = PhotoLoadType.Miniature)
        {
            var dig = new OpenFileDialog()
            {
                //TODO: Multi language support
                Title = "Chose the annotation file"
            };
            var photoLoader = new PhotoLoader();
            var annotationLoader = new AnnotationLoader();
            var (path, stream) = await _reader.Read(dig);
            try
            {
                var annotation = annotationLoader.Load(path, stream);
                var photoPath = Path.Combine(annotation.Folder, annotation.Filename);
                var photo = photoLoader.Load(photoPath, loadType);
                return new PhotoViewModel(photo, annotation);
            }
            catch (Exception e)
            {
                throw new Exception($"unable to read image from {path}");
            }
        }
        
        public async Task<PhotoViewModel[]> ReadAllFromDirByPhoto(PhotoLoadType loadType = PhotoLoadType.Miniature, bool isRecursive = false)
        {
            var dig = new OpenFolderDialog()
            {
                //TODO: Multi language support
                Title = "Chose directory image files"
            };
            var multipleFiles = await _reader.ReadAllFromDir(dig, isRecursive);
            var photoLoader = new PhotoLoader();
            var photoList = new List<PhotoViewModel>();
            foreach (var (path,stream) in multipleFiles)
            {
                try
                {
                    var photo = photoLoader.Load(path, stream, loadType);
                    var annotation = new Annotation
                    {
                        Filename = Path.GetFileName(path),
                        Folder = Path.GetDirectoryName(path)
                    };
                    photoList.Add(new PhotoViewModel(photo, annotation));
                }
                catch (Exception e)
                {
                    //TODO: translate to rus
                    Console.WriteLine($"ERROR: image from {path} is skipped!\nDetails: {e}");
                }
            }
            return photoList.ToArray();
        }
        
        public async Task<PhotoViewModel[]> ReadAllFromDirByAnnotation(PhotoLoadType loadType = PhotoLoadType.Miniature, bool isRecursive = false)
        {
            var dig = new OpenFolderDialog()
            {
                //TODO: Multi language support
                Title = "Chose directory image files"
            };
            var multipleFiles = await _reader.ReadAllFromDir(dig, isRecursive);
            var photoLoader = new PhotoLoader();
            var annotationLoader = new AnnotationLoader();
            var photoList = new List<PhotoViewModel>();
            foreach (var (path,stream) in multipleFiles)
            {
                try
                {
                    var annotation = annotationLoader.Load(path, stream);
                    var photoPath = Path.Combine(annotation.Folder, annotation.Filename);
                    var photo = photoLoader.Load(photoPath, loadType);
                    photoList.Add(new PhotoViewModel(photo, annotation));
                }
                catch (Exception e)
                {
                    //TODO: translate to rus
                    Console.WriteLine($"ERROR: image from {path} is skipped!\nDetails: {e}");
                }
            }
            return photoList.ToArray();
        }
    }
}