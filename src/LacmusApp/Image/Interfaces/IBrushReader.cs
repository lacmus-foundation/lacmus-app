using System.IO;

namespace LacmusApp.Image.Interfaces
{
    public interface IBrushReader<TBrush>
    {
        public (TBrush, int, int) Read(Stream stream);
    }
}