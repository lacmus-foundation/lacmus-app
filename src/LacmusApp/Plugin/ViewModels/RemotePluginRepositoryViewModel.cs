using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using LacmusApp.Plugin.Interfaces;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.ViewModels
{
    public class RemotePluginRepositoryViewModel : ReactiveObject, IRemotePluginRepositoryViewModel
    {
        private readonly ObservableAsPropertyHelper<IReadOnlyCollection<IObjectDetectionPlugin>> _plugins;
        private readonly ObservableAsPropertyHelper<string> _errorMessage;
        private readonly ObservableAsPropertyHelper<bool> _hasErrorMessage;
        
        public RemotePluginRepositoryViewModel(IPluginManager manager)
        {
            Refresh = ReactiveCommand
                .CreateFromTask(async ()  => await manager.GetPluginsFromRepository());
            
            InstallPlugin = ReactiveCommand
                .CreateFromTask<IObjectDetectionPlugin>(
                    async p =>
                    {
                        await manager.InstallPlugin(p);
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
                .Select(exception => $"can not get plugins: {exception.Message}")
                .ToProperty(this, x => x.ErrorMessage);
            
            _hasErrorMessage = InstallPlugin
                .ThrownExceptions
                .Select(exception => true)
                .ToProperty(this, x => x.HasErrorMessage);

            _errorMessage = InstallPlugin
                .ThrownExceptions
                .Select(exception => $"can not install plugin: {exception.Message}")
                .ToProperty(this, x => x.ErrorMessage);

            Refresh.Execute().Subscribe();
        }

        public ReactiveCommand<IObjectDetectionPlugin, Unit> InstallPlugin { get; }
        public ReactiveCommand<Unit, IReadOnlyCollection<IObjectDetectionPlugin>> Refresh { get; }
        public IReadOnlyCollection<IObjectDetectionPlugin> Plugins => _plugins.Value;
        public IObjectDetectionPlugin SelectedPlugin { get; set; }
        public string ErrorMessage => _errorMessage.Value;
        public bool HasErrorMessage => _hasErrorMessage.Value;
    }
}