using System.Collections.Generic;
using System.Threading.Tasks;
using RescuerLaApp.Models.Photo;

namespace RescuerLaApp.Services.Files
{
    public interface IPhotoWriter
    {
        Task Write(Photo photo);
        Task WriteMany(IEnumerable<Photo> photos);
    }
}