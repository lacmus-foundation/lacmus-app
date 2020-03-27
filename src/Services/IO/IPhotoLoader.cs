using LacmusApp.Models.Photo;

namespace LacmusApp.Services.IO
{
    public interface IPhotoLoader
    {
        Photo Load(string source, PhotoLoadType loadType);
    }
}