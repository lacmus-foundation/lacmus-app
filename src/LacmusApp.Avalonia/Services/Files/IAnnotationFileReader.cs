using System.Threading.Tasks;
using LacmusApp.Avalonia.Models;

namespace LacmusApp.Avalonia.Services.Files
{
    public interface IAnnotationFileReader
    {
        Task<Annotation> Read();
        Task<Annotation[]> ReadMultiple();
        Task<Annotation[]> ReadAllFromDir(bool isRecursive = false);
    }
}