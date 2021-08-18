using System.Collections.Generic;
using System.Threading.Tasks;
using LacmusApp.Avalonia.Models;

namespace LacmusApp.Avalonia.Services.Files
{
    public interface IAnnotationFileWriter
    {
        Task Write(Annotation annotation);
        Task WriteMany(IEnumerable<Annotation> annotations);
    }
}