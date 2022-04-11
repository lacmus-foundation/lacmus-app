using System;
using System.IO;
using System.Xml.Serialization;
using LacmusApp.Image.Interfaces;
using LacmusApp.Image.Models;

namespace LacmusApp.Image.Services
{
    public class AnnotationLoader : IAnnotationLoader
    {
        public Annotation ParseFromXml(Stream stream)
        {
            var formatter = new XmlSerializer(type:typeof(Annotation));
            try
            {
                return (Annotation)formatter.Deserialize(stream);
            }
            catch (Exception e)
            {
                throw new Exception($"unable serialize xml annotation.", e);
            }
        }
    }
}