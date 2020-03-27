using System.Collections.Generic;
using System.Threading.Tasks;
using LacmusApp.Models.Photo;

namespace LacmusApp.Services.Files
{
    public interface IPhotoWriter
    {
        Task Write(Photo photo);
        Task WriteMany(IEnumerable<Photo> photos);
    }
}