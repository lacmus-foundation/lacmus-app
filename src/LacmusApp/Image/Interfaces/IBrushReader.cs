using System.IO;
using System.Threading.Tasks;

namespace LacmusApp.Image.Interfaces
{
    public interface IBrushReader<TBrush>
    {
        public Task<(TBrush, int, int)> Read(Stream stream);
    }
}