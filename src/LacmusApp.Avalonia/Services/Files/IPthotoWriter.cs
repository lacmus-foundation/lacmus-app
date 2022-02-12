using System.Collections.Generic;
using System.Threading.Tasks;
using LacmusApp.Avalonia.Models.Photo;

namespace LacmusApp.Avalonia.Services.Files
{
    public interface IPhotoWriter
    {
        Task Write(Photo photo);
        Task WriteMany(IEnumerable<Photo> photos);
    }
}