using System.Threading.Tasks;
using RescuerLaApp.Models;

namespace RescuerLaApp.Services.Files
{
    public interface IAnnotationFileReader
    {
        Task<Annotation> Read();
        Task<Annotation[]> ReadMultiple();
        Task<Annotation[]> ReadAllFromDir(bool isRecursive = false);
    }
}