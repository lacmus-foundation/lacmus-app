using System.Collections.Generic;
using System.Collections.ObjectModel;
using LacmusApp.Plugin.Interfaces;
using LacmusApp.Screens.Interfaces;
using LacmusApp.Screens.Models;
using ReactiveUI;

namespace LacmusApp.Screens.ViewModels
{
    public class SettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        public SettingsViewModel(
            ILocalPluginRepositoryViewModel localPluginRepository,
            IRemotePluginRepositoryViewModel remotePluginRepository,
            IPluginInfoViewModel pluginInfo)
        {
            LocalPluginRepository = localPluginRepository;
            RemotePluginRepository = remotePluginRepository;
            PluginInfo = pluginInfo;
        }
        public ILocalPluginRepositoryViewModel LocalPluginRepository { get; }
        public IRemotePluginRepositoryViewModel RemotePluginRepository { get; }
        public IPluginInfoViewModel PluginInfo { get; }
    }
}