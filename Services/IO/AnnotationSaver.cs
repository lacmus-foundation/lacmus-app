using System;
using System.Xml.Serialization;
using RescuerLaApp.Models;

namespace RescuerLaApp.Services.IO
{
    public class AnnotationSaver : IAnnotationSaver
    {
        public void Save(Annotation annotation, string source)
        {
            var saver = new FileSaver();
            var formatter = new XmlSerializer(type:typeof(Annotation));
            using (var stream = saver.Save(source))
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