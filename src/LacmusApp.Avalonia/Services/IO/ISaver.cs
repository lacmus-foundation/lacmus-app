using System.IO;
using System.Threading.Tasks;

namespace LacmusApp.Avalonia.Services.IO
{
    public interface ISaver
    {
        Stream Save(string source);
    }
}