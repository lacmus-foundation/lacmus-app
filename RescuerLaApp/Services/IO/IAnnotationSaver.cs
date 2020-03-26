using RescuerLaApp.Models;

namespace RescuerLaApp.Services.IO
{
    public interface IAnnotationSaver
    {
        void Save(Annotation annotation, string source);
    }
}