using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Controls;
using LacmusApp.Interfaces;
using LacmusApp.Models;

namespace LacmusApp.Services.Files
{
    public class AvaloniaAnnotationFileWriter : IAnnotationFileWriter
    {
        private readonly Window _window;

        public AvaloniaAnnotationFileWriter(Window window)  => _window = window;
        
        public async Task Write(Annotation annotation)
        {
            var formatter = new XmlSerializer(type:typeof(Annotation));
            var writer = new AvaloniaFileWriter(_window);
            var folderDialog = new OpenFolderDialog
            {
                //TODO: Multi language support
                Title = "Select folder to save xml annotation."
            };
            try
            {
                using (var fs = await writer.Write($"{annotation.Filename}.xml", folderDialog))
                {
                    formatter.Serialize(fs, annotation);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to save annotation to {annotation.Filename}.xml", e);
            }
        }

        public async Task WriteMany(IEnumerable<Annotation> annotations)
        {
            var formatter = new XmlSerializer(type:typeof(Annotation));
            var folderDialog = new OpenFolderDialog
            {
                //TODO: Multi language support
                Title = "Select folder to save xml annotations."
            };
            var folder = await folderDialog.ShowAsync(_window);
            foreach (var annotation in annotations)
            {
                var path = Path.Combine(folder, $"{annotation.Filename}.xml");
                try
                {
                    using (var fs = new FileStream(path, FileMode.Create))
                    {
                        formatter.Serialize(fs, annotation);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Unable to save annotation to {annotation.Filename}.xml", e);
                }
            }
        }
    }
}