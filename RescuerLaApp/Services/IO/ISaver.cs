using System.IO;
using System.Threading.Tasks;

namespace RescuerLaApp.Services.IO
{
    public interface ISaver
    {
        Stream Save(string source);
    }
}