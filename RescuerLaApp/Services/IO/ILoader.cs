using System.IO;
using System.Threading.Tasks;

namespace RescuerLaApp.Services.IO
{
    public interface ILoader
    {
        Stream Load(string source);
    }
}