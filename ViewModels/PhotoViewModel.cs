using ReactiveUI;
using RescuerLaApp.Models;
using RescuerLaApp.Models.Photo;

namespace RescuerLaApp.ViewModels
{
    public class PhotoViewModel : ReactiveObject
    {
        public PhotoViewModel(Photo photo, Annotation annotation)
        {
            Photo = photo;
            Annotation = annotation;
        }
        public Photo Photo { get; set; }
        public Annotation Annotation { get; set; }
        public string Caption { get; set; }
        public string Path { get; set; }
        
        private static string GetCaptionFromPath(string path)
        {
            var name = System.IO.Path.GetFileName(path);
            if (name.Length > 10)
            {
                name = name.Substring(0, 3) + "{~}" + name.Substring(name.Length - 5);
            }
            return name;
        }
    }
}