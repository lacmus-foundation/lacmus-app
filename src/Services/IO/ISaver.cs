using System.IO;
using System.Threading.Tasks;

namespace LacmusApp.Services.IO
{
    public interface ISaver
    {
        Stream Save(string source);
    }
}