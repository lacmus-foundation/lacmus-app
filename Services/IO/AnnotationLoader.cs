using System;
using System.IO;
using System.Xml.Serialization;
using RescuerLaApp.Models;

namespace RescuerLaApp.Services.IO
{
    public class AnnotationLoader : IAnnotationLoader
    {
        public Annotation Load(string source)
        {
            var loader = new FileLoader();
            var formatter = new XmlSerializer(type:typeof(Annotation));
            using (var stream = loader.Load(source))
            {
                try
                {
                    return (Annotation)formatter.Deserialize(stream);
                }
                catch (Exception e)
                {
                    throw new Exception($"unable load xml annotation from {source}", e);
                }
            }
        }
        public Annotation Load(string source, Stream stream)
        {
            var formatter = new XmlSerializer(type:typeof(Annotation));
            using (stream)
            {
                try
                {
                    return (Annotation)formatter.Deserialize(stream);
                }
                catch (Exception e)
                {
                    throw new Exception($"unable load xml annotation from {source}", e);
                }
            }
        }
    }
}