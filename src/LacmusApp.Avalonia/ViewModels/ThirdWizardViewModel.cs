using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Services;
using LacmusApp.Avalonia.Services.Plugin;
using LacmusApp.Avalonia.Views;
using LacmusApp.Screens.ViewModels;
using Serilog;
using OperatingSystem = LacmusPlugin.OperatingSystem;

namespace LacmusApp.Avalonia.ViewModels
{
    public class ThirdWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly ApplicationStatusManager _applicationStatusManager;
        private WizardWindow _window;
        private SettingsViewModel _settingsViewModel;
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
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
        [Reactive] public string Error { get; set; }
        [Reactive] public bool IsError { get; set; } = false;
        [Reactive] public bool IsShowLoadModelButton { get; set; } = false;
        [Reactive] public LocalizationContext LocalizationContext { get; set; }
        
        public ReactiveCommand<Unit, Unit> LoadModelCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdateModelStatusCommand { get; }

        public ThirdWizardViewModel(IScreen screen, WizardWindow window, SettingsViewModel settingsViewModel, ApplicationStatusManager manager, LocalizationContext localizationContext)
        {
            _applicationStatusManager = manager;
            _window = window;
            LocalizationContext = localizationContext;
            HostScreen = screen;
            _settingsViewModel = settingsViewModel;
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

                var plugin = _settingsViewModel.Plugin;
                if (string.IsNullOrEmpty(plugin.Tag))
                    throw new Exception("No such plugin");
                
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
                IsError = false;
                Log.Information("Successfully loads ml model.");
            }
            catch (Exception e)
            {
                Status = $"Not ready.";
                IsError = true;
                Error = $"Error: {e.Message}";
                IsShowLoadModelButton = true;
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

                var plugin = _settingsViewModel.Plugin;
                if (string.IsNullOrEmpty(plugin.Tag))
                    throw new Exception("No such plugin");
                
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