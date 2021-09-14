using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using LacmusApp.Plugin.Interfaces;
using LacmusApp.Screens.Models;

namespace LacmusApp.Screens.Interfaces
{
    public interface ISettingsViewModel : INotifyPropertyChanged
    {
        ILocalPluginRepositoryViewModel LocalPluginRepository { get; }
        IRemotePluginRepositoryViewModel RemotePluginRepository { get; }
        IPluginInfoViewModel PluginInfo { get; }
    }
}