using LacmusApp.Models;

namespace LacmusApp.Services.IO
{
    public interface IAnnotationSaver
    {
        void Save(Annotation annotation, string source);
    }
}