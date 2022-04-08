using System.Collections.Generic;

namespace LacmusApp.Image.Interfaces
{
    public interface IImageLoader<out TBrush>
    {
        public IImage<TBrush> LoadFromFile(string path);
        public IEnumerable<IImage<TBrush>> LoadFromDirectory(string path);
        public IImage<TBrush> LoadFromXml(string path);
        public IEnumerable<IImage<TBrush>> LoadFromDirectoryWithXml(string path);
    }
}