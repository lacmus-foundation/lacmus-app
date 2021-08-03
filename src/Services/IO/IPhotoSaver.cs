using LacmusApp.Models.Photo;

namespace LacmusApp.Services.IO
{
    public interface IPhotoSaver
    {
        void Save(Photo photo, string source);
    }
}