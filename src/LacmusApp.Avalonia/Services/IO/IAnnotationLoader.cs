using LacmusApp.Avalonia.Models;

namespace LacmusApp.Avalonia.Services.IO
{
    public interface IAnnotationLoader
    {
        Annotation Load(string source);
    }
}