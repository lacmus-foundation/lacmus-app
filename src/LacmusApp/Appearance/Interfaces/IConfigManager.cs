using System.Threading.Tasks;

namespace LacmusApp.Appearance.Interfaces
{
    public interface IConfigManager
    {
        Task<IConfig> ReadConfig();
        Task SaveConfig(IConfig config);
    }
}