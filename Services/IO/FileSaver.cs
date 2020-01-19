using System.IO;
using System.Threading.Tasks;

namespace RescuerLaApp.Services.IO
{
    public class FileSaver : ISaver
    {
        public Stream Save(string source)
        {
            return File.Create(source);
        }
    }
}