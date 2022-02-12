using System.Threading.Tasks;

namespace LacmusApp.IO.Interfaces
{
    public interface IFileManager
    {
        Task<string> SelectFileToWrite();
        Task<string> SelectFileToRead();
    }
}