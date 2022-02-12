using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using LacmusApp.Plugin.Interfaces;
using ReactiveUI;
using Serilog;

namespace LacmusApp.Plugin.ViewModels
{
    public class RemotePluginRepositoryViewModel : ReactiveObject, IRemotePluginRepositoryViewModel
    {
        private readonly ObservableAsPropertyHelper<IReadOnlyCollection<IRemotePluginViewModel>> _plugins;
        private readonly ObservableAsPropertyHelper<string> _errorMessage;
        private readonly ObservableAsPropertyHelper<bool> _hasErrorMessage;
        
        public RemotePluginRepositoryViewModel(IPluginManager manager)
        {
            Refresh = ReactiveCommand
                .CreateFromTask<IReadOnlyCollection<IRemotePluginViewModel>>(async ()  =>
                { 
                    var list = await manager.GetPluginsFromRepository();
                    return list.Select(p => new RemotePluginViewModel(p, manager)).ToList();
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
        public ReactiveCommand<Unit, IReadOnlyCollection<IRemotePluginViewModel>> Refresh { get; }
        public IReadOnlyCollection<IRemotePluginViewModel> Plugins => _plugins.Value;
        public string ErrorMessage => _errorMessage.Value;
        public bool HasErrorMessage => _hasErrorMessage.Value;
    }
}