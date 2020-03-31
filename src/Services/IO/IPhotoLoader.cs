using System.Threading.Tasks;
using LacmusApp.Models.Photo;

namespace LacmusApp.Services.IO
{
    public interface IPhotoLoader
    {
        Task<Photo> Load(string source, PhotoLoadType loadType);
    }
}