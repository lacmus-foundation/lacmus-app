using RescuerLaApp.Models.Photo;

namespace RescuerLaApp.Services.IO
{
    public interface IPhotoSaver
    {
        void Save(Photo photo, string source);
    }
}