using System.Collections.Generic;
using LacmusApp.Models;
using LacmusApp.ViewModels;

namespace LacmusApp.Extensions
{
    public static class PhotoViewModelExtension
    {
        public static List<BoundBox> GetBoundingBoxes(this PhotoViewModel photoViewModel)
        {
            List<BoundBox> list = new List<BoundBox>();
            foreach (var annotationObject in photoViewModel.Annotation.Objects)
            {
                list.Add(new BoundBox(
                    x: annotationObject.Box.Xmin,
                    y: annotationObject.Box.Ymin,
                    width: annotationObject.Box.Xmax - annotationObject.Box.Xmin,
                    height: annotationObject.Box.Ymax - annotationObject.Box.Ymin
                ));
            }

            return list;
        }
    }
}