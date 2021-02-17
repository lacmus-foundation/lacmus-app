using System;
using System.IO;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Managers;
using LacmusApp.Models;
using LacmusApp.Models.ML;
using LacmusApp.Services;
using LacmusApp.Views;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class ThirdWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly string _mlConfigPath = Path.Join("conf", "mlConfig.json");
        private readonly ApplicationStatusManager _applicationStatusManager;
        private WizardWindow _window;
        private AppConfig _appConfig;
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        [Reactive] public string Repository { get; set; } = "None";
        [Reactive] public string Type { get; set; } = "None";
        [Reactive] public string Version { get; set; } = "None";
        [Reactive] public string Status { get; set; } = "Not ready";
        [Reactive] public string Error { get; set; }
        [Reactive] public bool IsError { get; set; } = false;
        [Reactive] public bool IsShowLoadModelButton { get; set; } = false;
        [Reactive] public LocalizationContext LocalizationContext { get; set; }
        
        public ReactiveCommand<Unit, Unit> LoadModelCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateModelStatusCommand { get; }

        public ThirdWizardViewModel(IScreen screen, WizardWindow window, ApplicationStatusManager manager, LocalizationContext localizationContext)
        {
            _applicationStatusManager = manager;
            _window = window;
            LocalizationContext = localizationContext;
            HostScreen = screen;
            _appConfig = window.AppConfig;
            LoadModelCommand = ReactiveCommand.Create(LoadModel);
            UpdateModelStatusCommand = ReactiveCommand.Create(UpdateModelStatus);
        }

        public async void UpdateModelStatus()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");
            //get the last version of ml model with specific config
            try
            {
                Log.Information("Loading ml model.");
                Status = "Loading ml model...";
                var confDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lacmus");
                var configPath = Path.Join(confDir,"appConfig.json");
                _appConfig = await AppConfig.Create(configPath);
                var config = _appConfig.MlModelConfig;;
                // get local versions
                var localVersions = await MLModel.GetInstalledVersions(config);
                if(localVersions.Contains(config.ModelVersion))
                {
                    Log.Information($"Find local version: {config.Image.Name}:{config.Image.Tag}.");
                }
                else
                {
                    IsShowLoadModelButton = true;
                    throw new Exception($"There are no ml local models to init: {config.Image.Name}:{config.Image.Tag}");
                }
                Repository = config.Image.Name;
                Version = $"{config.ModelVersion}";
                Type = $"{config.Type}";
                using(var model = new MLModel(config))
                    await model.Download();
                Status = $"Ready";
                IsError = false;
                Log.Information("Successfully loads ml model.");
            }
            catch (Exception e)
            {
                Status = $"Not ready.";
                IsError = true;
                Error = $"Error: {e.Message}";
                Log.Error(e, "Unable to load model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private async void LoadModel()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");
            //get the last version of ml model with specific config
            try
            {
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                ModelManagerWindow window = new ModelManagerWindow(_window.LocalizationContext, ref _appConfig, _applicationStatusManager, _window.ThemeManager);
                _appConfig = await window.ShowResult();
                var config = _appConfig.MlModelConfig;
                // init local model or download and init it from docker registry
                var localVersions = await MLModel.GetInstalledVersions(config);
                if(localVersions.Contains(config.ModelVersion))
                {
                    Log.Information($"Find local version: {config.Image.Name}:{config.Image.Tag}.");
                }
                else
                {
                    IsShowLoadModelButton = true;
                    throw new Exception($"There are no ml local models to init: {config.Image.Name}:{config.Image.Tag}");
                }
                Repository = config.Image.Name;
                Version = $"{config.ModelVersion}";
                Type = $"{config.Type}";
                using(var model = new MLModel(config))
                    await model.Download();
                Status = $"Ready";
                IsError = false;
                _window.AppConfig = _appConfig;
            }
            catch (Exception e)
            {
                Status = $"Not ready.";
                IsError = true;
                Error = $"Error: {e.Message}";
                Log.Error(e, "Unable to load model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
    }
}