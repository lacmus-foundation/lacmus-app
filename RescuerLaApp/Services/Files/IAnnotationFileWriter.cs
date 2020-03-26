using System.Collections.Generic;
using System.Threading.Tasks;
using RescuerLaApp.Models;

namespace RescuerLaApp.Services.Files
{
    public interface IAnnotationFileWriter
    {
        Task Write(Annotation annotation);
        Task WriteMany(IEnumerable<Annotation> annotations);
    }
}