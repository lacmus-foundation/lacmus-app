using System;
using System.Collections.ObjectModel;
using System.Reactive;
using DynamicData;
using LacmusApp.Managers;
using LacmusApp.Models;
using LacmusApp.Models.ML;
using LacmusApp.Services.Files;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class ModelManagerWindowViewModel : ReactiveObject
    {
        LocalizationContext LocalizationContext { get; set; }
        private readonly ApplicationStatusManager _applicationStatusManager;
        private AppConfig _config;
        
        private SourceList<MlModelData> _avalableModels { get; set; } = new SourceList<MlModelData>();
        private ReadOnlyObservableCollection<MlModelData> _avalableModelsCollection;
        public ReadOnlyObservableCollection<MlModelData> AvalableModelsCollection => _avalableModelsCollection;
        
        private SourceList<MlModelData> _installedModels { get; set; } = new SourceList<MlModelData>();
        private ReadOnlyObservableCollection<MlModelData> _installedModelsCollection;
        public ReadOnlyObservableCollection<MlModelData> InstalledModelsCollection => _installedModelsCollection;
        
        public ModelManagerWindowViewModel(LocalizationContext context,
                                        AppConfig config,
                                        ApplicationStatusManager manager)
        {
            this.LocalizationContext = context;
            _config = config;
            _applicationStatusManager = manager;
            
            _avalableModels
                .Connect()
                .Bind(out _avalableModelsCollection)
                .Subscribe();
            
            _installedModels
                .Connect()
                .Bind(out _installedModelsCollection)
                .Subscribe();
            
            UpdateModelStatusCommand = ReactiveCommand.Create(UpdateModelStatus);
            UpdateInstalledModelsCommand = ReactiveCommand.Create(UpdateInstalledModels);
            UpdateAvailableModelsCommand = ReactiveCommand.Create(UpdateAvailableModels);
            
            UpdateModelStatusCommand.Execute().Subscribe();
            UpdateInstalledModelsCommand.Execute().Subscribe();
            UpdateAvailableModelsCommand.Execute().Subscribe();
        }
        
        public ReactiveCommand<Unit, Unit> UpdateModelStatusCommand { get; set; }
        public ReactiveCommand<Unit, Unit> UpdateInstalledModelsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> UpdateAvailableModelsCommand { get; set; }
        
        [Reactive] public string Repository { get; set; } = "None";
        [Reactive] public string Type { get; set; } = "None";
        [Reactive] public string Version { get; set; } = "None";
        [Reactive] public string Status { get; set; } = "Not ready";
        
        public async void UpdateModelStatus()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");
            //get the last version of ml model with specific config
            try
            {
                Log.Information("Loading ml model.");
                Status = "Loading ml model...";
                var config = _config.MlModelConfig;;
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
                Log.Error(e, "Unable to init model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
        public async void UpdateInstalledModels()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");
            try
            {
                Log.Information("Get installed ml models.");
                var models = await MLModel.GetInstalledModels();
                foreach (var model in models)
                {
                    _installedModels.Add(new MlModelData(model.Image.Name,
                        model.Type,
                        model.ModelVersion,
                        model.ApiVersion));
                }
                Log.Information("Successfully get installed ml models.");
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to get installed models.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
        public async void UpdateAvailableModels()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");
            try
            {
                Log.Information("Get ml models from registry.");
                foreach (var repository in _config.Repositories)
                {
                    try
                    {
                        Console.WriteLine(repository);
                        var models = await MLModel.GetAvailableModelsFromRegistry(repository);
                        foreach (var model in models)
                        {
                            _avalableModels.Add(new MlModelData(model.Image.Name,
                                model.Type,
                                model.ModelVersion,
                                model.ApiVersion));
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e,$"Unable to parse models from {repository}. Skipped.");
                    }
                }
                Log.Information("Successfully get available ml models.");
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable get available models.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
    }
}