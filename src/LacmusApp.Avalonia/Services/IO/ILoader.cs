using System.IO;
using System.Threading.Tasks;

namespace LacmusApp.Avalonia.Services.IO
{
    public interface ILoader
    {
        Stream Load(string source);
    }
}