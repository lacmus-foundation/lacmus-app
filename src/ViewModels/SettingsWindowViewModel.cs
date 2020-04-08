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
using LacmusApp.Managers;
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
        public SettingsWindowViewModel(LocalizationContext context, ThemeManager mainThemeManager, ThemeManager settingsThemeManager)
        {
            this.LocalizationContext = context;
            _settingsThemeManager = settingsThemeManager;
            _mainThemeManager = mainThemeManager;

            this.WhenAnyValue(x => x.ThemeIndex)
                .Skip(1)
                .Subscribe(x => SwitchSettingsTheme());
            
            SetupCommands();
        }

        public ReactiveCommand<Unit, Unit> ApplyCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }

        [Reactive] public int LanguageIndex { get; set; } = 0;
        [Reactive] public int ThemeIndex { get; set; } = 0;

        private void SetupCommands()
        {
            ApplyCommand = ReactiveCommand.Create(Apply);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }
        
        private void Apply()
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
            }
            catch (Exception e)
            {
                Log.Error("Unable to apply settings", e);
            }
        }

        private void Cancel()
        {
            
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
