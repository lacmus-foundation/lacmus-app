using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using LacmusApp.Appearance.Enums;
using LacmusApp.Appearance.Models;
using LacmusApp.Plugin.Interfaces;
using ReactiveUI;

namespace LacmusApp.Screens.Interfaces
{
    public interface ISettingsViewModel : INotifyPropertyChanged
    {
        ReactiveCommand<Unit, Config> Apply { get; }
        ReactiveCommand<Unit, Config> Cancel { get; }
        ILocalPluginRepositoryViewModel LocalPluginRepository { get; }
        IRemotePluginRepositoryViewModel RemotePluginRepository { get; }
        IPluginViewModel Plugin { get; set; }
        public float PredictionThreshold { get; set; }
        public string PluginsRepositoryUrl { get; set; }
        public Language Language { get; set; }
        public IEnumerable<Language> SupportedLanguages { get; }
        public Theme Theme { get; set; }
        public IEnumerable<Theme> SupportedThemes { get; }
        public BoundingBoxColour BoundingBoxColour { get; set; }
        public IEnumerable<BoundingBoxColour> SupportedBoundingBoxColours { get; }
        public bool IsNeedRestart { get; set; }
    }
}