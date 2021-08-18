using System.Threading.Tasks;
using LacmusApp.Avalonia.Models.Photo;

namespace LacmusApp.Avalonia.Services.Files
{
    public interface IPhotoReader
    {
        Task<Photo> Read(PhotoLoadType loadType = PhotoLoadType.Miniature);
        Task<Photo[]> ReadMultiple(PhotoLoadType loadType = PhotoLoadType.Miniature);
        Task<Photo[]> ReadAllFromDir(PhotoLoadType loadType = PhotoLoadType.Miniature, bool isRecursive = false);
    }
}