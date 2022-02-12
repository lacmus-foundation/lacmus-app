using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using LacmusApp.IO.Interfaces;
using LacmusApp.Plugin.Interfaces;
using LacmusApp.Screens.Interfaces;
using ReactiveUI;
using Serilog;

namespace LacmusApp.Plugin.ViewModels
{
    public class LocalPluginRepositoryViewModel : ReactiveObject, ILocalPluginRepositoryViewModel
    {
        private readonly ObservableAsPropertyHelper<IReadOnlyCollection<ILocalPluginViewModel>> _plugins;
        private readonly ObservableAsPropertyHelper<string> _errorMessage;
        private readonly ObservableAsPropertyHelper<bool> _hasErrorMessage;

        public LocalPluginRepositoryViewModel(IPluginManager manager, IFileManager file, ISettingsViewModel settings)
        {
            Refresh = ReactiveCommand
                .CreateFromTask<IReadOnlyCollection<ILocalPluginViewModel>>(async ()  =>
                {
                    var list = await manager.GetInstalledPlugins();
                    return list.Select(p => new LocalPluginViewModel(p, manager, settings)).ToList();
                });
            
            Import = ReactiveCommand
                .CreateFromTask(async ()  =>
                {
                    var path = await file.SelectFileToRead();
                    await manager.ImportPlugin(path);
                });

            _plugins = Refresh
                .Select(p => p)
                .ToProperty(this, x => x.Plugins);
            
            _hasErrorMessage = Refresh
                .ThrownExceptions
                .Select(exception => true)
                .ToProperty(this, x => x.HasErrorMessage);

            _errorMessage = Refresh
                .ThrownExceptions
                .Select(exception =>
                {
                    Log.Error(exception.Message, exception);
                    return $"can not get plugins: {exception.Message}";
                })
                .ToProperty(this, x => x.ErrorMessage);
        }
        
        public ReactiveCommand<Unit, IReadOnlyCollection<ILocalPluginViewModel>> Refresh { get; }
        public ReactiveCommand<Unit, Unit> Import { get; }
        public IReadOnlyCollection<ILocalPluginViewModel> Plugins => _plugins.Value;
        public string ErrorMessage => _errorMessage.Value;
        public bool HasErrorMessage => _hasErrorMessage.Value;
    }
}