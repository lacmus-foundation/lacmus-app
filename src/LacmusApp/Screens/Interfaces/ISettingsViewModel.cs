using System.Collections.Generic;
using System.ComponentModel;
using LacmusApp.Appearance.Enums;
using LacmusApp.Plugin.Interfaces;

namespace LacmusApp.Screens.Interfaces
{
    public interface ISettingsViewModel : INotifyPropertyChanged
    {
        ILocalPluginRepositoryViewModel LocalPluginRepository { get; }
        IRemotePluginRepositoryViewModel RemotePluginRepository { get; }
        IPluginViewModel Plugin { get; }
        public float PredictionThreshold { get; set; }
        public string PluginsRepositoryUrl { get; set; }
        public Language Language { get; set; }
        public IEnumerable<Language> SupportedLanguages { get; }
        public Theme Theme { get; set; }
        public IEnumerable<Theme> SupportedThemes { get; }
        public BoundingBoxColour BoundingBoxColour { get; set; }
        public IEnumerable<BoundingBoxColour> SupportedBoundingBoxColours { get; }
    }
}