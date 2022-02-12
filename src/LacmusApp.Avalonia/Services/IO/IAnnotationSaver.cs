using LacmusApp.Avalonia.Models;

namespace LacmusApp.Avalonia.Services.IO
{
    public interface IAnnotationSaver
    {
        void Save(Annotation annotation, string source);
    }
}