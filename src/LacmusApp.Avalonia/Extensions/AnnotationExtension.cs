using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LacmusApp.Image.Models;
using LacmusPlugin;
using Annotation = LacmusApp.Avalonia.Models.Annotation;

namespace LacmusApp.Avalonia.Extensions
{
    public static class AnnotationExtension
    {
        public static string GetPath(this Annotation annotation)
        {
            return Path.Combine(annotation.Folder, annotation.Filename);
        }
        
        public static IEnumerable<IObject> GetDetections(this Annotation annotation)
        {
            return annotation.Objects.Select(x => new Detection
            {
                Label = x.Name,
                Score = 1.0f,
                XMax = x.Box.Xmax,
                XMin = x.Box.Xmin,
                YMax = x.Box.Ymax,
                YMin = x.Box.Ymin
            });
        }
    }
}