using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using LacmusApp.Appearance.Enums;
using LacmusApp.Appearance.Interfaces;
using LacmusApp.Appearance.Models;
using LacmusApp.IO.Interfaces;
using LacmusApp.Plugin.Interfaces;
using LacmusApp.Plugin.Models;
using LacmusApp.Plugin.ViewModels;
using LacmusApp.Screens.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace LacmusApp.Screens.ViewModels
{
    //TODO: add error message
    public class SettingsViewModel : ReactiveObject, ISettingsViewModel
    {
        private readonly ObservableAsPropertyHelper<bool> _isNeedRestart;
        public SettingsViewModel(
            Config config,
            IConfigManager configManager,
            IPluginManager pluginManager,
            IFileManager fileManager)
        {
            LocalPluginRepository = new LocalPluginRepositoryViewModel(pluginManager, fileManager, this);
            RemotePluginRepository = new RemotePluginRepositoryViewModel(pluginManager);
            Plugin = new PluginViewModel(config.Plugin, pluginManager);
            PredictionThreshold = config.PredictionThreshold;
            PluginsRepositoryUrl = config.Repository;
            Language = config.Language;
            Theme = config.Theme;
            BoundingBoxColour = config.BoundingBoxColour;
            
            var oldConfig = new Config()
            {
                Language = this.Language,
                Plugin = new PluginInfo()
                {
                    Author = this.Plugin.Author,
                    Company = this.Plugin.Company,
                    Dependences = this.Plugin.Dependences,
                    Description = this.Plugin.Description,
                    Name = this.Plugin.Name,
                    Tag = this.Plugin.Tag,
                    Url = this.Plugin.Url,
                    Version = this.Plugin.Version,
                    InferenceType = this.Plugin.InferenceType,
                    OperatingSystems = this.Plugin.OperatingSystems
                },
                Repository = this.PluginsRepositoryUrl,
                Theme = this.Theme,
                PredictionThreshold = this.PredictionThreshold,
                BoundingBoxColour = this.BoundingBoxColour
            };

            Apply = ReactiveCommand
                .CreateFromTask(async () =>
                {
                    var newConfig = new Config()
                    {
                        Language = this.Language,
                        Plugin = new PluginInfo()
                        {
                            Author = this.Plugin.Author,
                            Company = this.Plugin.Company,
                            Dependences = this.Plugin.Dependences,
                            Description = this.Plugin.Description,
                            Name = this.Plugin.Name,
                            Tag = this.Plugin.Tag,
                            Url = this.Plugin.Url,
                            Version = this.Plugin.Version,
                            InferenceType = this.Plugin.InferenceType,
                            OperatingSystems = this.Plugin.OperatingSystems
                        },
                        Repository = this.PluginsRepositoryUrl,
                        Theme = this.Theme,
                        PredictionThreshold = this.PredictionThreshold,
                        BoundingBoxColour = this.BoundingBoxColour
                    };
                    await configManager.SaveConfig(newConfig);
                    OnRequestClose?.Invoke(this, EventArgs.Empty);
                    if (IsNeedRestart)
                        OnRequestRestart?.Invoke(this, EventArgs.Empty);
                    return newConfig;
                });
            
            Cancel = ReactiveCommand
                .Create(() =>
                {
                    Plugin = new PluginViewModel(oldConfig.Plugin, pluginManager);
                    PredictionThreshold = oldConfig.PredictionThreshold;
                    PluginsRepositoryUrl = oldConfig.Repository;
                    Language = oldConfig.Language;
                    Theme = oldConfig.Theme;
                    BoundingBoxColour = oldConfig.BoundingBoxColour;
                    OnRequestClose?.Invoke(this, EventArgs.Empty);
                    return oldConfig;
                });

            var isThemeChanged = this.WhenAnyValue(x => x.Theme)
                .Select(x => x != oldConfig.Theme);
            var isLanguageChanged = this.WhenAnyValue(x => x.Language)
                .Select(x => x != oldConfig.Language);
            var isPluginChanged = this.WhenAnyValue(x => x.Plugin)
                .Select(x => $"{x.Tag}-{x.Version.ToString()}" != $"{oldConfig.Plugin.Tag}-{oldConfig.Plugin.Version.ToString()}");
            
            _isNeedRestart = Observable
                .Merge(isThemeChanged, isLanguageChanged, isPluginChanged)
                .ToProperty(this, x => x.IsNeedRestart);
            
            Log.Debug($"{PredictionThreshold}");
        }

        public ReactiveCommand<Unit, Config> Apply { get; }
        public ReactiveCommand<Unit, Config> Cancel { get; }
        public ILocalPluginRepositoryViewModel LocalPluginRepository { get; }
        public IRemotePluginRepositoryViewModel RemotePluginRepository { get; }
        [Reactive] public IPluginViewModel Plugin { get; set; }
        public float PredictionThreshold { get; set; }
        public string PluginsRepositoryUrl { get; set; }
        [Reactive] public Language Language { get; set; }
        [Reactive] public Theme Theme { get; set; }
        public BoundingBoxColour BoundingBoxColour { get; set; }
        public IEnumerable<Language> SupportedLanguages => new []
        {
            Language.English,
            Language.Russian
        };
        public IEnumerable<Theme> SupportedThemes => new[] {Theme.Light, Theme.Dark};
        public IEnumerable<BoundingBoxColour> SupportedBoundingBoxColours => new[]
        {
            BoundingBoxColour.Red,
            BoundingBoxColour.Green,
            BoundingBoxColour.Blue,
            BoundingBoxColour.Cyan,
            BoundingBoxColour.Yellow,
            BoundingBoxColour.Magenta
        };
        public bool IsNeedRestart => _isNeedRestart.Value;
        public event EventHandler OnRequestClose;
        public event EventHandler OnRequestRestart;
    }
}