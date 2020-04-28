using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Logging.Serilog;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using LacmusApp.Managers;
using LacmusApp.Models;
using LacmusApp.Models.ML;
using MetadataExtractor;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Services.Files;
using LacmusApp.Views;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class SettingsWindowViewModel : ReactiveObject
    {
        LocalizationContext LocalizationContext {get; set;}
        private ThemeManager _settingsThemeManager, _mainThemeManager;
        private readonly ApplicationStatusManager _applicationStatusManager;
        private AppConfig _config, _newConfig;
        private SettingsWindow _window;
        
        public SettingsWindowViewModel(SettingsWindow window, LocalizationContext context,
                                        ref AppConfig config,
                                        ApplicationStatusManager manager,
                                        ThemeManager mainThemeManager,
                                        ThemeManager settingsThemeManager)
        {
            _window = window;
            this.LocalizationContext = context;
            _settingsThemeManager = settingsThemeManager;
            _mainThemeManager = mainThemeManager;
            _config = config;
            _newConfig = AppConfig.DeepCopy(_config);
            _applicationStatusManager = manager;

            this.WhenAnyValue(x => x.ThemeIndex)
                .Skip(1)
                .Subscribe(x => SwitchSettingsTheme());
            
            SetupCommands();

            UpdateModelStatusCommand.Execute().Subscribe();
        }

        public ReactiveCommand<Unit, Unit> ApplyCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> UpdateModelStatusCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenModelMnagerCommand { get; set; }

        [Reactive] public int LanguageIndex { get; set; } = 0;
        [Reactive] public int ThemeIndex { get; set; } = 0;
        [Reactive] public string HexColor { get; set; } = "#FFFF0000";
        [Reactive] public string Repository { get; set; } = "None";
        [Reactive] public string Type { get; set; } = "None";
        [Reactive] public string Version { get; set; } = "None";
        [Reactive] public string Status { get; set; } = "Not ready";
        private void SetupCommands()
        {
            ApplyCommand = ReactiveCommand.Create(Apply);
            CancelCommand = ReactiveCommand.Create(Cancel);
            UpdateModelStatusCommand = ReactiveCommand.Create(UpdateModelStatus);
            OpenModelMnagerCommand = ReactiveCommand.Create(OpenModelManager);
        }
        
        private async void Apply()
        {
            try
            {
                switch (LanguageIndex)
                {
                    case 0:
                        LocalizationContext.Language = Language.English;
                        break;
                    case 1:
                        LocalizationContext.Language = Language.Russian;
                        break;
                    default:
                        throw new Exception($"Invalid LanguageIndex: {LanguageIndex}");
                }
                _mainThemeManager.UseTheme(_settingsThemeManager.CurrentTheme);
                
                //save app settings
                _newConfig.Language = LocalizationContext.Language;
                _newConfig.Theme = _mainThemeManager.CurrentTheme;
                _newConfig.BorderColor = HexColor;
                //TODO: ml config settings
                
                await _newConfig.Save();
                _config = AppConfig.DeepCopy(_newConfig);
                _window.AppConfig = _config;
                _window.Close();
            }
            catch (Exception e)
            {
                Log.Error("Unable to apply settings", e);
            }
        }

        private void Cancel()
        {
            _window.Close();
        }
        public async void UpdateModelStatus()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");
            //get the last version of ml model with specific config
            try
            {
                Log.Information("Loading ml model.");
                Status = "Loading ml model...";
                var config = _newConfig.MlModelConfig;;
                // get local versions
                var localVersions = await MLModel.GetInstalledVersions(config);
                if(!localVersions.Contains(config.ModelVersion))
                    throw new Exception($"There are no ml local model to init: {config.Image.Name}:{config.Image.Tag}");
                
                Repository = config.Image.Name;
                Version = $"{config.ModelVersion}";
                Type = $"{config.Type}";
                Status = $"Ready";
                Log.Information("Successfully init ml model.");
            }
            catch (Exception e)
            {
                Status = $"Not ready.";
                Log.Error(e, "Unable to load model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private async void OpenModelManager()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
            var window = new ModelManagerWindow(LocalizationContext, ref _newConfig, _applicationStatusManager, _mainThemeManager);
            _newConfig = await window.ShowResult();
        }
        private void SwitchSettingsTheme()
        {
            try
            {
                switch (ThemeIndex)
                {
                    case 0:
                        _settingsThemeManager.UseTheme(ThemeManager.Theme.Citrus);
                        break;
                    case 1:
                        _settingsThemeManager.UseTheme(ThemeManager.Theme.Rust);
                        break;
                    case 2:
                        _settingsThemeManager.UseTheme(ThemeManager.Theme.Sea);
                        break;
                    case 3:
                        _settingsThemeManager.UseTheme(ThemeManager.Theme.Candy);
                        break;
                    case 4:
                        _settingsThemeManager.UseTheme(ThemeManager.Theme.Magma);
                        break;
                    default:
                        throw new Exception($"Invalid ThemeIndex: {LanguageIndex}");
                }
            }
            catch (Exception e)
            {
                Log.Error("Unable to change theme.", e);
            }
        }
    }
}
