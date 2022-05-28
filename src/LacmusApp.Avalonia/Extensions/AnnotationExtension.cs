using System.Collections.Generic;
using System.IO;
using System.Linq;
using LacmusApp.Image.Models;
using LacmusPlugin;

namespace LacmusApp.Avalonia.Extensions
{
    public static class AnnotationExtension
    {
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