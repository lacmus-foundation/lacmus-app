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
using MetadataExtractor;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Services.Files;
using LacmusApp.Views;

namespace LacmusApp.ViewModels
{
    public class SettingsWindowViewModel : ReactiveObject
    {
        LocalizationContext LocalizationContext {get; set;}
        [Reactive] public string CurrentLanguage
        {
            get; set;
        }
        public SettingsWindowViewModel(LocalizationContext context)
        {
            this.LocalizationContext = context;
            SetupCommands();
        }

        public ReactiveCommand<Unit, Unit> ChangeRussianCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ChangeEnglishCommand { get; set; }

        private void SetupCommands()
        {
            ChangeRussianCommand = ReactiveCommand.Create(ChangeRussian);
            ChangeEnglishCommand = ReactiveCommand.Create(ChangeEnglish);
        }
        
        private void ChangeRussian()
        {
            LocalizationContext.Language = Language.Russian;
        }

        private void ChangeEnglish()
        {
            LocalizationContext.Language = Language.English;
        }

    }
    
}
