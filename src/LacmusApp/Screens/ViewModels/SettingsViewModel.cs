using System;
using LacmusApp.Plugin.Interfaces;
using LacmusApp.Screens.Interfaces;
using ReactiveUI;

namespace LacmusApp.Screens.ViewModels
{
    public class SettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        public SettingsViewModel(
            ILocalPluginRepositoryViewModel localPluginRepository,
            IRemotePluginRepositoryViewModel remotePluginRepository,
            IPluginViewModel plugin)
        {
            LocalPluginRepository = localPluginRepository;
            RemotePluginRepository = remotePluginRepository;
            Plugin = plugin;
            
            // initialize components
            Plugin.Activate.Execute().Subscribe();
            RemotePluginRepository.Refresh.Execute().Subscribe();
        }
        public ILocalPluginRepositoryViewModel LocalPluginRepository { get; }
        public IRemotePluginRepositoryViewModel RemotePluginRepository { get; }
        public IPluginViewModel Plugin { get; }
    }
}