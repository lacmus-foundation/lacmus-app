using System;
using System.IO;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Extensions;
using LacmusApp.Managers;
using LacmusApp.Models;
using LacmusApp.Models.ML;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class ThirdWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly string _mlConfigPath = Path.Join("conf", "mlConfig.json");
        private readonly ApplicationStatusManager _applicationStatusManager;
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
        
        public ReactiveCommand<Unit, Unit> LoadModelCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateModelStatusCommand { get; }

        public ThirdWizardViewModel(IScreen screen, ApplicationStatusManager manager, AppConfig config)
        {
            _applicationStatusManager = manager;
            _appConfig = config;
            HostScreen = screen;
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
                var config = _appConfig.MlModelConfig;;
                // get local versions
                var localVersions = await MLModel.GetInstalledVersions(config);
                if (localVersions.Any())
                {
                    config.ModelVersion = localVersions.Max();
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
                Log.Information("Check ml model status.");
                var config = _appConfig.MlModelConfig;;
                // get local versions
                var localVersions = await MLModel.GetInstalledVersions(config);
                if (localVersions.Any())
                {
                    config.ModelVersion = localVersions.Max();
                    Log.Information($"Find local version: {config.Image.Name}:{config.Image.Tag}.");
                }
                else
                {
                    // if there are no local models try to download it from docker registry
                    var netVersions = await MLModel.GetAvailableVersionsFromRegistry(config);
                    if (netVersions.Any())
                    {
                        config.ModelVersion = netVersions.Max();
                        Log.Information($"Find version in registry: {config.Image.Name}:{config.Image.Tag}.");
                    }
                    else
                    {
                        throw new Exception($"There are no ml models to init: {config.Image.Name}:{config.Image.Tag}");
                    }
                }
                await config.Save(_mlConfigPath);
                // init local model or download and init it from docker registry
                using(var model = new MLModel(config))
                    await model.Download();
                
                await config.Save(_mlConfigPath);
                Repository = config.Image.Name;
                Version = $"{config.ModelVersion}";
                Type = $"{config.Type}";
                using(var model = new MLModel(config))
                    await model.Download();
                Status = $"Ready";
                IsError = false;
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