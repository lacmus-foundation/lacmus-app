using System;
using System.IO;
using System.Xml.Serialization;
using LacmusApp.Avalonia.Models;

namespace LacmusApp.Avalonia.Services.IO
{
    public class AnnotationSaver : IAnnotationSaver
    {
        public void Save(Annotation annotation, string source)
        {
          
            var formatter = new XmlSerializer(type:typeof(Annotation));
            using (var stream = File.Create(source))
            {
                try
                {
                    formatter.Serialize(stream, annotation);
                }
                catch (Exception e)
                {
                    throw new Exception($"unable save xml annotation to {source}", e);
                }
            }
        }
    }
}