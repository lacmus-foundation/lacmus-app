using RescuerLaApp.Models;

namespace RescuerLaApp.Services.IO
{
    public interface IAnnotationLoader
    {
        Annotation Load(string source);
    }
}