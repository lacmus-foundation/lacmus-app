using RescuerLaApp.Models.Photo;

namespace RescuerLaApp.Services.IO
{
    public interface IPhotoLoader
    {
        Photo Load(string source, PhotoLoadType loadType);
    }
}