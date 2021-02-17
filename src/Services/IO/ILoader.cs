using System.IO;
using System.Threading.Tasks;

namespace LacmusApp.Services.IO
{
    public interface ILoader
    {
        Stream Load(string source);
    }
}