using System.Threading.Tasks;
using LacmusApp.Models;

namespace LacmusApp.Services.Files
{
    public interface IAnnotationFileReader
    {
        Task<Annotation> Read();
        Task<Annotation[]> ReadMultiple();
        Task<Annotation[]> ReadAllFromDir(bool isRecursive = false);
    }
}