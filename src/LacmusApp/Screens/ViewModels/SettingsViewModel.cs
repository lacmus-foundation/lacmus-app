using System;
using System.Collections.Generic;
using LacmusApp.Appearance.Enums;
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
        public float PredictionThreshold { get; set; }
        public string PluginsRepositoryUrl { get; set; } = "http://api.lacmus.ml";
        public Language Language { get; set; }
        public IEnumerable<Language> SupportedLanguages => new [] {Language.English, Language.Russian};
        public Theme Theme { get; set; }
        public IEnumerable<Theme> SupportedThemes => new[] {Theme.Light, Theme.Dark};
        public BoundingBoxColour BoundingBoxColour { get; set; }

        public IEnumerable<BoundingBoxColour> SupportedBoundingBoxColours => new[]
        {
            BoundingBoxColour.Red,
            BoundingBoxColour.Green,
            BoundingBoxColour.Blue,
            BoundingBoxColour.Cyan,
            BoundingBoxColour.Yellow,
            BoundingBoxColour.Magenta
        };
    }
}