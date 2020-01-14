using System.Threading.Tasks;
using Avalonia.Controls;
using RescuerLaApp.Models;

namespace RescuerLaApp.Interfaces
{
    public interface IAnnotationFileReader
    {
        Task<Annotation> Read();
        Task<Annotation[]> ReadMultiple();
        Task<Annotation[]> ReadAllFromDir(bool isRecursive = false);
    }
}