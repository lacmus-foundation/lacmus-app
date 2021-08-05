using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Controls;
using LacmusApp.Models;

namespace LacmusApp.Services.Files
{
    public class AvaloniaAnnotationFileReader : IAnnotationFileReader
    {
        private readonly AvaloniaFileReader _reader;

        public AvaloniaAnnotationFileReader(Window window)  => _reader = new AvaloniaFileReader(window);
        
        public async Task<Annotation> Read()
        {
            var formatter = new XmlSerializer(type:typeof(Annotation));
            var dig = new OpenFileDialog()
            {
                //TODO: Multi language support
                Title = "Chose xml annotation file"
            };
            var (path, stream) = await _reader.Read(dig);
            try
            {
                using (stream)
                {
                    return (Annotation)formatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"unable serialize xml annotation {path}");
            }
        }

        public async Task<Annotation[]> ReadMultiple()
        {
            var dig = new OpenFileDialog()
            {
                //TODO: Multi language support
                Title = "Chose xml annotation files"
            };
            var multipleFiles = await _reader.ReadMultiple(dig);
            return GetAnnotationsFromFiles(multipleFiles);
        }

        public async Task<Annotation[]> ReadAllFromDir(bool isRecursive = false)
        {
            var dig = new OpenFolderDialog()
            {
                //TODO: Multi language support
                Title = "Chose directory xml annotation files"
            };
            var multipleFiles = await _reader.ReadAllFromDir(dig, isRecursive);
            return GetAnnotationsFromFiles(multipleFiles);
        }

        private Annotation[] GetAnnotationsFromFiles(IEnumerable<(string Path, Stream Stream)> multipleFiles)
        {
            var formatter = new XmlSerializer(type:typeof(Annotation));
            var annotations = new List<Annotation>();
            foreach (var (path, stream) in multipleFiles)
            {
                try
                {
                    using (stream)
                    {
                        annotations.Add((Annotation)formatter.Deserialize(stream));
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"unable serialize xml annotation {path}", e);
                }
            }
            return annotations.ToArray();
        }
    }
}