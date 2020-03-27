using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using MetadataExtractor;
using LacmusApp.Extensions;
using LacmusApp.Models;
using LacmusApp.Models.Photo;
using LacmusApp.Services.Files;
using LacmusApp.Services.IO;
using LacmusApp.ViewModels;
using Serilog;
using Attribute = LacmusApp.Models.Photo.Attribute;
using ProgressBar = Avalonia.Controls.ProgressBar;

namespace LacmusApp.Services.VM
{
    public class PhotoVMReader
    {
        private readonly AvaloniaFileReader _reader;
        
        public PhotoVMReader(Window window)  => _reader = new AvaloniaFileReader(window);
        
        public async Task<PhotoViewModel> ReadByPhoto(int id, PhotoLoadType loadType = PhotoLoadType.Miniature)
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
                return new PhotoViewModel(id, photo, annotation);
            }
            catch (Exception e)
            {
                throw new Exception($"unable to read image from {path}");
            }
        }
        
        public async Task<PhotoViewModel> ReadByAnnotation(int id, PhotoLoadType loadType = PhotoLoadType.Miniature)
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
                return new PhotoViewModel(id, photo, annotation);
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
            var count = 0;
            var id = 0;
            var valueTuples = multipleFiles as (string Path, Stream Stream)[] ?? multipleFiles.ToArray();
            using (var pb = new Models.ProgressBar())
            {
                foreach (var (path, stream) in valueTuples)
                {
                    try
                    {
                        using (stream)
                        {
                            if (Path.GetExtension(path).ToLower() != ".jpg" &&
                                Path.GetExtension(path).ToLower() != ".jpeg" &&
                                Path.GetExtension(path).ToLower() != ".png")
                            {
                                count++;
                                continue;
                            }
                            
                            var photo = photoLoader.Load(path, stream, loadType);
                            var annotation = new Annotation
                            {
                                Filename = Path.GetFileName(path),
                                Folder = Path.GetDirectoryName(path)
                            };
                            var photoViewModel = new PhotoViewModel(id, photo, annotation);
                            photoViewModel.BoundBoxes = photoViewModel.GetBoundingBoxes();
                            photoList.Add(photoViewModel);
                            id++;
                            count++;
                            pb.Report((double)count / valueTuples.Count(), $"Processed {count} of {valueTuples.Count()}");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e,$"image from {path} is skipped!");
                    }
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
            var count = 0;
            var id = 0;
            var valueTuples = multipleFiles as (string Path, Stream Stream)[] ?? multipleFiles.ToArray();
            using (var pb = new Models.ProgressBar())
            {
                foreach (var (path,stream) in valueTuples)
                {
                    try
                    {
                        if (Path.GetExtension(path).ToLower() != ".xml")
                        {
                            count++;
                            continue;
                        }
                        
                        var annotation = annotationLoader.Load(path, stream);
                        var photoPath = Path.Combine(annotation.Folder, annotation.Filename);
                        var photo = photoLoader.Load(photoPath, loadType);
                        var photoViewModel = new PhotoViewModel(id, photo, annotation);
                        photoViewModel.BoundBoxes = photoViewModel.GetBoundingBoxes();
                        photoList.Add(photoViewModel);
                        id++;
                        count++;
                        pb.Report((double)count / valueTuples.Count(), $"Processed {count} of {valueTuples.Count()}");
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e,$"image from {path} is skipped!");
                    }
                }
            }
            return photoList.ToArray();
        }
    }
}