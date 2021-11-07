using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using LacmusApp.IO.Interfaces;
using LacmusApp.Plugin.Interfaces;
using ReactiveUI;
using Serilog;

namespace LacmusApp.Plugin.ViewModels
{
    public class LocalPluginRepositoryViewModel : ReactiveObject, ILocalPluginRepositoryViewModel
    {
        private readonly ObservableAsPropertyHelper<IReadOnlyCollection<IPluginViewModel>> _plugins;
        private readonly ObservableAsPropertyHelper<string> _errorMessage;
        private readonly ObservableAsPropertyHelper<bool> _hasErrorMessage;

        public LocalPluginRepositoryViewModel(IPluginManager manager, IFileManager file)
        {
            Refresh = ReactiveCommand
                .CreateFromTask<IReadOnlyCollection<IPluginViewModel>>(async ()  =>
                {
                    var list = await manager.GetInstalledPlugins();
                    return list.Select(p => new PluginViewModel(p, manager)).ToList();
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
        
        public ReactiveCommand<Unit, IReadOnlyCollection<IPluginViewModel>> Refresh { get; }
        public ReactiveCommand<Unit, Unit> Import { get; }
        public IReadOnlyCollection<IPluginViewModel> Plugins => _plugins.Value;
        public string ErrorMessage => _errorMessage.Value;
        public bool HasErrorMessage => _hasErrorMessage.Value;
    }
}