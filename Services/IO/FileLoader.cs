using System.IO;
using System.Threading.Tasks;

namespace RescuerLaApp.Services.IO
{
    public class FileLoader : ILoader
    {
        public Stream Load(string source)
        {
            return File.OpenRead(source);
        }
    }
}