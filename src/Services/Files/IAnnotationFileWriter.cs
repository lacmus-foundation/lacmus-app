using System.Collections.Generic;
using System.Threading.Tasks;
using LacmusApp.Models;

namespace LacmusApp.Services.Files
{
    public interface IAnnotationFileWriter
    {
        Task Write(Annotation annotation);
        Task WriteMany(IEnumerable<Annotation> annotations);
    }
}