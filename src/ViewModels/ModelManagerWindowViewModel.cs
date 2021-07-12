using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using LacmusApp.Managers;
using LacmusApp.Models;
using LacmusApp.Models.Plugins;
using LacmusApp.Services;
using LacmusApp.Services.Plugin;
using LacmusApp.Views;
using LacmusPlugin;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;
using OperatingSystem = LacmusPlugin.OperatingSystem;

namespace LacmusApp.ViewModels
{
    public class ModelManagerWindowViewModel : ReactiveValidationObject<ModelManagerWindowViewModel>
    {
        private const uint API_VERSION = 1;
        private PluginManager _pluginManager;
        LocalizationContext LocalizationContext { get; set; }
        private readonly ApplicationStatusManager _applicationStatusManager;
        private AppConfig _config, _newConfig;

        private SourceList<PluginInfo> _avalableModels { get; set; } = new SourceList<PluginInfo>();
        private ReadOnlyObservableCollection<PluginInfo> _avalableModelsCollection;
        public ReadOnlyObservableCollection<PluginInfo> AvalableModelsCollection => _avalableModelsCollection;
        
        private SourceList<PluginInfo> _installedModels { get; set; } = new SourceList<PluginInfo>();
        private ReadOnlyObservableCollection<PluginInfo> _installedModelsCollection;
        public ReadOnlyObservableCollection<PluginInfo> InstalledModelsCollection => _installedModelsCollection;
        
        private SourceList<PluginRepository> _repositories { get; set; } = new SourceList<PluginRepository>();
        private ReadOnlyObservableCollection<PluginRepository> _repositoriesCollection;
        public ReadOnlyObservableCollection<PluginRepository> RepositoriesCollection => _repositoriesCollection;
        
        public ModelManagerWindowViewModel(ModelManagerWindow window, LocalizationContext context,
                                        ref AppConfig config,
                                        ApplicationStatusManager manager)
        {
            this.LocalizationContext = context;
            _config = config;
            _newConfig = AppConfig.DeepCopy(_config);
            _pluginManager = new PluginManager(_newConfig.PluginDir);
            
            _applicationStatusManager = manager;
            
            _avalableModels
                .Connect()
                .Bind(out _avalableModelsCollection)
                .Subscribe();
            
            _installedModels
                .Connect()
                .Bind(out _installedModelsCollection)
                .Subscribe();
            
            _repositories
                .Connect()
                .Bind(out _repositoriesCollection)
                .Subscribe();

            //var repoRule = this.ValidationRule(
            //    viewModel => viewModel.RepositoryToAdd,
            //    x => string.IsNullOrWhiteSpace(x) == false,
            //    x => $"Incorrect repository {x}");

            UpdateModelStatusCommand = ReactiveCommand.Create(async () => { await UpdateModelStatus(); }, CanExecute());
            UpdateInstalledModelsCommand = ReactiveCommand.Create(async () => { await UpdateInstalledModels(); }, CanExecute());
            UpdateAvailableModelsCommand = ReactiveCommand.Create(async () => { await UpdateAvailableModels(); }, CanExecute());
            DownloadModelCommand = ReactiveCommand.Create(async () => { await DownloadModel(); }, CanExecute());
            RemoveModelCommand = ReactiveCommand.Create(async () => { await RemoveModel(); }, CanExecute());
            ActivateModelCommand = ReactiveCommand.Create(async () => { await ActivateModel(); }, CanExecute());
            AddRepositoryCommand = ReactiveCommand.Create(AddRepository, this.IsValid());
            RemoveRepositoryCommand = ReactiveCommand.Create(RemoveRepository, CanExecute());
            
            ApplyCommand = ReactiveCommand.Create(async () =>
            {
                _config = AppConfig.DeepCopy(_newConfig);
                await _config.Save();
                window.AppConfig = _config;
                window.Close(); 
            }, CanExecute());
            
            CancelCommand = ReactiveCommand.Create(window.Close, CanExecute());
            
            Task.Run(Init);
        }
        private IObservable<bool> CanExecute()
        {
            return _applicationStatusManager.AppStatusInfoObservable
                .Select(status => status.Status != Enums.Status.Working && status.Status != Enums.Status.Unauthenticated);
        }
        
        public ReactiveCommand<Unit, Task> UpdateModelStatusCommand { get; set; }
        public ReactiveCommand<Unit, Task> UpdateInstalledModelsCommand { get; set; }
        public ReactiveCommand<Unit, Task> UpdateAvailableModelsCommand { get; set; }
        public ReactiveCommand<Unit, Task> DownloadModelCommand { get; set; }
        public ReactiveCommand<Unit, Task> RemoveModelCommand { get; set; }
        public ReactiveCommand<Unit, Task> ActivateModelCommand { get; set; }
        public ReactiveCommand<Unit, Task> ApplyCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> AddRepositoryCommand { get; set; }
        public ReactiveCommand<Unit, Unit> RemoveRepositoryCommand { get; set; }
        
        [Reactive] public string Name { get; set; } = "None";
        [Reactive] public string Author { get; set; } = "None";
        [Reactive] public string Company { get; set; } = "None";
        [Reactive] public string Description { get; set; } = "None";
        [Reactive] public string Tag { get; set; } = "None";
        [Reactive] public string InferenceType { get; set; } = "None";
        [Reactive] public string Version { get; set; } = "None";
        [Reactive] public string Url { get; set; } = "None";
        [Reactive] public string OperatingSystems { get; set; } = "None";


        [Reactive] public string Status { get; set; } = "Not ready";
        [Reactive] public PluginInfo SelectedAvailableModel { get; set; } = null;
        [Reactive] public PluginInfo SelectedInstalledModel { get; set; } = null;
        [Reactive] public string SelectedRepository { get; set; } = null;
        [Reactive] public string RepositoryToAdd { get; set; }

        public async void Init()
        {
            Log.Information("init...");
            //get model status
            try
            {
                Status = "Loading ml model...";
                var config = _newConfig;
                // get local versions
                var installedPlugins = await _pluginManager.GetInstalledPluginsAsync();
                
                foreach (var plugin in installedPlugins)
                {
                    if (plugin.Tag == config.PluginInfo.Tag && plugin.Version.ToString() == config.PluginInfo.Version.ToString())
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            Name = plugin.Name;
                            Author = plugin.Author;
                            Company = plugin.Company;
                            Description = plugin.Description;
                            Tag = plugin.Tag;
                            InferenceType = plugin.InferenceType.ToString();
                            Version = plugin.Version.ToString();
                            Url = plugin.Url;
                            OperatingSystems = ConvertOperatingSystemsToString(plugin.OperatingSystems);
                            Status = $"Ready";
                        });
                    }
                    Dispatcher.UIThread.Post(() => _installedModels.Add(new PluginInfo(plugin)));
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable get installed mdels.");
            }
            //get available models
            try
            {
                foreach (var repository in _newConfig.Repositories)
                    try
                    {
                        var plugins = await _pluginManager.GetPluginsFromRepositoryAsync(repository);
                        Dispatcher.UIThread.Post(() =>
                        {
                            _repositories.Add(repository);
                            foreach (var plugin in plugins)
                            {
                                _avalableModels.Add(new PluginInfo(plugin));
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e,$"Unable to parse models from {repository}. Skipped.");
                    }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable get available models.");
            }
            
            Dispatcher.UIThread.Post(() =>
            {
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
            });
        }
        
        public async Task UpdateModelStatus()
        {
            var config = _newConfig;
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");
            //get the last version of ml model with specific config
            try
            {
                Log.Information("Loading ml model.");
                Status = "Loading ml model...";
                var plugin = await _pluginManager.GetPluginsAsync(config.PluginInfo.Tag, config.PluginInfo.Version);
                
                Dispatcher.UIThread.Post(() =>
                {
                    Name = plugin.Name;
                    Author = plugin.Author;
                    Company = plugin.Company;
                    Description = plugin.Description;
                    Tag = plugin.Tag;
                    InferenceType = plugin.InferenceType.ToString();
                    Version = plugin.Version.ToString();
                    Url = plugin.Url;
                    OperatingSystems = ConvertOperatingSystemsToString(plugin.OperatingSystems);
                    Status = $"Ready";
                });
                Log.Information("Successfully init ml model.");
            }
            catch (Exception e)
            {
                Status = $"Not ready.";
                Log.Error(e, "Unable to init model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
        public async Task UpdateInstalledModels()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | get installed models...");
            _installedModels.Clear();
            try
            {
                Log.Information("Get installed ml models.");
                // get local versions
                var installedPlugins = await _pluginManager.GetInstalledPluginsAsync();
                
                foreach (var plugin in installedPlugins)
                {
                    Dispatcher.UIThread.Post(() => _installedModels.Add(new PluginInfo(plugin)));
                }
                Log.Information("Successfully get installed ml models.");
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to get installed models.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
        public async Task UpdateAvailableModels()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | get available models...");
            try
            {
                Log.Information("Get available ml models from registry.");
                _avalableModels.Clear();
                foreach (var repository in _newConfig.Repositories)
                {
                    try
                    {
                        var plugins = await _pluginManager.GetPluginsFromRepositoryAsync(repository);
                        Dispatcher.UIThread.Post(() =>
                        {
                            foreach (var plugin in plugins)
                            {
                                _avalableModels.Add(new PluginInfo(plugin));
                            }
                        });
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
        public async Task DownloadModel()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");
            try
            {
                if(SelectedAvailableModel == null)
                    throw new Exception("No selected model.");
                await _pluginManager.InstallPlugin(SelectedAvailableModel, _newConfig.Repositories.First());
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to download ml model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
        public async Task RemoveModel()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | remove model...");
            try
            {
                if(SelectedInstalledModel == null)
                    throw new Exception("No selected model.");
                
                Directory.Delete(Path.Combine(_newConfig.PluginDir, SelectedInstalledModel.Tag, SelectedInstalledModel.Version.ToString()), true);

                if (SelectedInstalledModel.Tag == Tag &&
                    Version == SelectedInstalledModel.Version.ToString())
                {
                    Name = "None";
                    Author = "None";
                    Company = "None";
                    Description = "None";
                    Tag = "None";
                    InferenceType = "None";
                    Version = "None";
                    Url = "None";
                    OperatingSystems = "None";
                    Status = $"Not ready";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to remove ml model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
        public async Task ActivateModel()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | activate model...");
            
            try
            {
                if(SelectedInstalledModel == null)
                    throw new Exception("No selected model.");

                _newConfig.PluginInfo = new PluginInfo(SelectedInstalledModel);

                await UpdateModelStatus();
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to activate ml model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        public void AddRepository()
        {
            try
            {
                //_repositories.Add(RepositoryToAdd);
                //var repoList = _newConfig.Repositories.ToList();
                //repoList.Add(RepositoryToAdd);
                //_newConfig.Repositories = repoList.ToArray();
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to add repository");
            }
        }

        public void RemoveRepository()
        {
            try
            {
                //_repositories.Remove(SelectedRepository);
                //var repoList = _newConfig.Repositories.ToList();
                //repoList.Remove(RepositoryToAdd);
                //_newConfig.Repositories = repoList.ToArray();
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to remove repository");
            }
        }
        
        private string ConvertOperatingSystemsToString(IEnumerable<OperatingSystem> operatingSystems)
        {
            var result = "";
            foreach (var os in operatingSystems)
            {
                switch (os)
                {
                    case OperatingSystem.AndroidArm:
                        result += "Android";
                        break;
                    case OperatingSystem.IosArm:
                        result += "IOS";
                        break;
                    case OperatingSystem.LinuxAmd64:
                        result += "Linux";
                        break;
                    case OperatingSystem.LinuxArm:
                        result += "Linux (ARM)";
                        break;
                    case OperatingSystem.OsxAmd64:
                        result += "OSX (amd64)";
                        break;
                    case OperatingSystem.OsxArm:
                        result += "OSX (Apple Silicon)";
                        break;
                    case OperatingSystem.WindowsAmd64:
                        result += "Windows";
                        break;
                    case OperatingSystem.WindowsArm:
                        result += "Windows (ARM)";
                        break;
                    default:
                        result += os.ToString();
                        break;
                }
                result += ";";
            }

            return result;
        }
    }
}