using LacmusApp.Models;

namespace LacmusApp.Services.IO
{
    public interface IAnnotationLoader
    {
        Annotation Load(string source);
    }
}