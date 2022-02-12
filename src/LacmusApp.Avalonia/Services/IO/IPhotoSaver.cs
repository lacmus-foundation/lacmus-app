using LacmusApp.Avalonia.Models.Photo;

namespace LacmusApp.Avalonia.Services.IO
{
    public interface IPhotoSaver
    {
        void Save(Photo photo, string source);
    }
}