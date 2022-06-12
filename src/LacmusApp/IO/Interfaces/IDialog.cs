using System.Threading.Tasks;

namespace LacmusApp.IO.Interfaces
{
    public interface IDialog
    {
        Task<string> SelectToWrite();
        Task<string> SelectToRead();
    }
}