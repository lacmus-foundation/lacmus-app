using System.Threading.Tasks;
using LacmusApp.Appearance.Models;

namespace LacmusApp.Appearance.Interfaces
{
    public interface IConfigManager
    {
        Task<Config> ReadConfig();
        Task SaveConfig(Config config);
    }
}