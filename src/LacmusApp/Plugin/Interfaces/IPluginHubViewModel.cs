using System.ComponentModel;

namespace LacmusApp.Plugin.Interfaces
{
    public interface IPluginHubViewModel : INotifyPropertyChanged
    {
        IPluginInfoViewModel PluginInfoViewModel { get; }
    }
}