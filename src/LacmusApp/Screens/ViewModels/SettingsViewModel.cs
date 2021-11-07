using System;
using System.Collections.Generic;
using System.Reactive;
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

namespace LacmusApp.Screens.ViewModels
{
    //TODO: add error message
    public class SettingsViewModel : ReactiveObject, ISettingsViewModel
    {
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
                    return newConfig;
                });
            
            Cancel = ReactiveCommand
                .Create(  () => config);

            // TODO: init when activate VM
            // initialize components
            try
            {
                Plugin.Activate.Execute().Subscribe();
                // not work witjout internwt
                // RemotePluginRepository.Refresh.Execute().Subscribe();
            }
            catch
            {
                // ignored
            }
        }

        public ReactiveCommand<Unit, Config> Apply { get; }
        public ReactiveCommand<Unit, Config> Cancel { get; }
        public ILocalPluginRepositoryViewModel LocalPluginRepository { get; }
        public IRemotePluginRepositoryViewModel RemotePluginRepository { get; }
        [Reactive] public IPluginViewModel Plugin { get; set; }
        public float PredictionThreshold { get; set; }
        public string PluginsRepositoryUrl { get; set; }
        public Language Language { get; set; }
        public Theme Theme { get; set; }
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
        public bool IsNeedRestart { get; set; } = false;
    }
}