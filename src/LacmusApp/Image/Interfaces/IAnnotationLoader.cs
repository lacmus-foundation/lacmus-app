using System.IO;
using LacmusApp.Image.Models;

namespace LacmusApp.Image.Interfaces
{
    public interface IAnnotationLoader
    {
        public Annotation ParseFromXml(Stream stream);
    }
}