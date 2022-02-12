using System.IO;
using System.Text.RegularExpressions;
using LacmusApp.Avalonia.Models;

namespace LacmusApp.Avalonia.Extensions
{
    public static class AnnotationExtension
    {
        public static string GetCaption(this Annotation annotation)
        {
            var name = Path.GetFileNameWithoutExtension(annotation.Filename);
            if (name.Length <= 15)
                return name;
            
            var digitName = Regex.Replace(name, @"[^\d]", "");
            if (!string.IsNullOrWhiteSpace(digitName))
                name = digitName;
                
            if (name.Length > 15)
            {
                name = name.Substring(0, 3) + "{~}" + name.Substring(name.Length - 10);
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