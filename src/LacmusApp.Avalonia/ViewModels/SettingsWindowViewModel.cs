using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Threading;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Services;
using LacmusApp.Avalonia.Services.Plugin;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Avalonia.Views;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using Serilog;
using OperatingSystem = LacmusPlugin.OperatingSystem;

namespace LacmusApp.Avalonia.ViewModels
{
    public class SettingsWindowViewModel : ReactiveObject
    {
        LocalizationContext LocalizationContext {get; set;}
        private ThemeManager _mainThemeManager;
        private readonly ApplicationStatusManager _applicationStatusManager;
        private AppConfig _config, _newConfig;
        private SettingsWindow _window;
        private bool isThemeChanged = false;
        private ThemeManager.Theme _currentTheme;
        
        public SettingsWindowViewModel(SettingsWindow window, LocalizationContext context,
                                        ref AppConfig config,
                                        ApplicationStatusManager manager,
                                        ThemeManager mainThemeManager)
        {
            _window = window;
            this.LocalizationContext = context;
            _mainThemeManager = mainThemeManager;
            _config = config;
            _newConfig = AppConfig.DeepCopy(_config);
            _currentTheme = _config.Theme;
            _applicationStatusManager = manager;
            InitView();

            this.WhenAnyValue(x => x.ThemeIndex)
                .Skip(1)
                .Subscribe(x => SwitchSettingsTheme());
            
            SetupCommands();

            MLUrl = "_newConfig.MlModelConfig.Url";

            UpdateModelStatusCommand.Execute().Subscribe();
        }

        public ReactiveCommand<Unit, Unit> ApplyCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> UpdateModelStatusCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenModelMnagerCommand { get; set; }

        [Reactive] public int LanguageIndex { get; set; } = 0;
        [Reactive] public int ThemeIndex { get; set; } = 0;
        [Reactive] public string HexColor { get; set; } = "#FFFF0000";
        
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
        [Reactive] public string MLUrl { get; set; } = "http://localhost:5000";
        private void SetupCommands()
        {
            ApplyCommand = ReactiveCommand.Create(Apply);
            CancelCommand = ReactiveCommand.Create(Cancel);
            UpdateModelStatusCommand = ReactiveCommand.Create(UpdateModelStatus);
            OpenModelMnagerCommand = ReactiveCommand.Create(OpenModelManager);
        }

        private void InitView()
        {
            switch (LocalizationContext.Language)
            {
                case Language.English:
                    LanguageIndex = 0;
                    break;
                case Language.Russian:
                    LanguageIndex = 1;
                    break;
            }

            switch (_currentTheme)
            {
                case ThemeManager.Theme.Citrus:
                    ThemeIndex = 0;
                    break;
                case ThemeManager.Theme.Rust:
                    ThemeIndex = 1;
                    break;
                case ThemeManager.Theme.Sea:
                    ThemeIndex = 2;
                    break;
                case ThemeManager.Theme.Candy:
                    ThemeIndex = 3;
                    break;
                case ThemeManager.Theme.Magma:
                    ThemeIndex = 4;
                    break;
            }
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
                
                //save app settings
                _newConfig.Language = LocalizationContext.Language;
                _newConfig.Theme = _currentTheme;
                _newConfig.BorderColor = HexColor;
                
                await _newConfig.Save();
                _config = AppConfig.DeepCopy(_newConfig);
                _window.AppConfig = _config;
                if (isThemeChanged)
                {
                    //TODO: refactor it
                    var msg = "To use new theme you need to restart application.";
                    if (LocalizationContext.Language == Language.Russian)
                        msg = "Чтобы изменить тему интерфейса необходим перезапуск программы.";
                    var msgbox = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
                    {
                        ButtonDefinitions = ButtonEnum.Ok,
                        ContentTitle = "Need to restart",
                        ContentMessage = msg,
                        Icon = MessageBox.Avalonia.Enums.Icon.Info,
                        Style = Style.None,
                        ShowInCenter = true
                    });
                    var result = await msgbox.Show();
                }
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
                Status = "Loading ml model...";
                var pluginManager = new PluginManager(_newConfig.PluginDir);
                var config = _newConfig;
                var plugin = await pluginManager.GetPluginsAsync(config.PluginInfo.Tag, config.PluginInfo.Version);
                
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
                        _currentTheme = ThemeManager.Theme.Citrus;
                        break;
                    case 1:
                        _currentTheme = ThemeManager.Theme.Rust;
                        break;
                    case 2:
                        _currentTheme = ThemeManager.Theme.Sea;
                        break;
                    case 3:
                        _currentTheme = ThemeManager.Theme.Candy;
                        break;
                    case 4:
                        _currentTheme = ThemeManager.Theme.Magma;
                        break;
                    default:
                        throw new Exception($"Invalid ThemeIndex: {LanguageIndex}");
                }
                isThemeChanged = true;
            }
            catch (Exception e)
            {
                Log.Error("Unable to change theme.", e);
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
