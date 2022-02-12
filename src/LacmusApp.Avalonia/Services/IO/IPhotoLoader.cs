using System.Threading.Tasks;
using LacmusApp.Avalonia.Models.Photo;

namespace LacmusApp.Avalonia.Services.IO
{
    public interface IPhotoLoader
    {
        Task<Photo> Load(string source, PhotoLoadType loadType);
    }
}