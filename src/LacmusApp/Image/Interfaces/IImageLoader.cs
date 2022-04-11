using System.Collections.Generic;
using System.Threading.Tasks;

namespace LacmusApp.Image.Interfaces
{
    public interface IImageLoader<TBrush>
    {
        public Task<IImage<TBrush>> LoadFromFile(string path);
        public Task<IEnumerable<IImage<TBrush>>> LoadFromDirectory(string path, bool isRecursive = false);
        public Task<IImage<TBrush>> LoadFromXml(string path);
        public Task<IEnumerable<IImage<TBrush>>> LoadFromDirectoryWithXml(string path, bool isRecursive = false);
    }
}