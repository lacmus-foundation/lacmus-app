using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using LacmusApp.Plugin.Interfaces;
using LacmusApp.Screens.Interfaces;
using LacmusPlugin;
using LacmusPlugin.Enums;
using ReactiveUI;
using OperatingSystem = LacmusPlugin.OperatingSystem;
using Version = LacmusPlugin.Version;

namespace LacmusApp.Plugin.ViewModels
{
    public class LocalPluginViewModel : ReactiveObject, ILocalPluginViewModel
    {
        private IObjectDetectionPlugin _plugin;
        private readonly ObservableAsPropertyHelper<string> _errorMessage;
        private readonly ObservableAsPropertyHelper<bool> _hasErrorMessage;
        
        public LocalPluginViewModel(IObjectDetectionPlugin plugin, IPluginManager manager, ISettingsViewModel settings)
        {
            _plugin = plugin;
            
            Remove = ReactiveCommand.CreateFromTask(
                async () => await manager.UninstallPlugin(plugin));
            
            Activate = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    _plugin = await manager.LoadPlugin(plugin.Tag, plugin.Version);
                    settings.Plugin = this;
                    return _plugin;
                });

            Activate
                .Select(p => p.Tag)
                .ToProperty(this, x => x.Tag);
            
            Activate
                .Select(p => p.Name)
                .ToProperty(this, x => x.Name);
            
            Activate
                .Select(p => p.Author)
                .ToProperty(this, x => x.Author);
            
            Activate
                .Select(p => p.Company)
                .ToProperty(this, x => x.Company);
            
            Activate
                .Select(p => p.Dependences)
                .ToProperty(this, x => x.Dependences);
            
            Activate
                .Select(p => p.Url)
                .ToProperty(this, x => x.Url);
            
            Activate
                .Select(p => p.Version)
                .ToProperty(this, x => x.Version);
            
            Activate
                .Select(p => p.InferenceType)
                .ToProperty(this, x => x.InferenceType);
            
            Activate
                .Select(p => p.OperatingSystems)
                .ToProperty(this, x => x.OperatingSystems);
            
            var hasErrorMessageRemove = Remove
                .ThrownExceptions
                .Select(_ => true);

            var errorMessageRemove = Remove
                .ThrownExceptions
                .Select(_ => "Can not uninstall plugin.");

            var hasErrorMessageActivate = Activate
                .ThrownExceptions
                .Select(_ => true);

            var errorMessageActivate = Activate
                .ThrownExceptions
                .Select(_ => "Can not activate plugin.");

            _hasErrorMessage = Observable
                .Merge(hasErrorMessageRemove, hasErrorMessageActivate)
                .ToProperty(this, x => x.HasErrorMessage);
            
            _errorMessage = Observable
                .Merge(errorMessageRemove, errorMessageActivate)
                .ToProperty(this, x => x.ErrorMessage);
        }
        
        public IObjectDetectionModel LoadModel(float threshold)
        {
            return _plugin.LoadModel(threshold);
        }
        
        public string Tag => _plugin.Tag;
        public string Name => _plugin.Name;
        public string Description => _plugin.Description;
        public string Author => _plugin.Author;
        public string Company => _plugin.Company;
        public IEnumerable<string> Dependences => _plugin.Dependences;
        public string Url => _plugin.Url;
        public Version Version => _plugin.Version;
        public InferenceType InferenceType => _plugin.InferenceType;
        public HashSet<OperatingSystem> OperatingSystems => _plugin.OperatingSystems;
        public ReactiveCommand<Unit, IObjectDetectionPlugin> Activate { get; }
        public ReactiveCommand<Unit, Unit> Remove { get; }
        public string ErrorMessage => _errorMessage.Value;
        public bool HasErrorMessage => _hasErrorMessage.Value;
    }
}