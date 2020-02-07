using System.IO;
using RescuerLaApp.Models;

namespace RescuerLaApp.Extensions
{
    public static class AnnotationExtension
    {
        public static string GetCaption(this Annotation annotation)
        {
            var name = annotation.Filename;
            if (name.Length > 10)
            {
                name = name.Substring(0, 3) + "{~}" + name.Substring(name.Length - 5);
            }
            return name;
        }
        
        public static string GetPhotoPath(this Annotation annotation)
        {
            return Path.Combine(annotation.Folder, annotation.Filename);
        }
        
        //TODO: GetRectangle(...)
    }
}