using System;
using System.Reactive.Linq;
using LacmusApp.Plugin.Extensions;
using LacmusApp.Plugin.Interfaces;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.ViewModels
{
    public class PluginInfoViewModel : ReactiveObject, IPluginInfoViewModel
    {
        private readonly ObservableAsPropertyHelper<string> _errorMessage;
        private readonly ObservableAsPropertyHelper<bool> _hasErrorMessage;
        private readonly ObservableAsPropertyHelper<string> _tag;
        private readonly ObservableAsPropertyHelper<string> _name;
        private readonly ObservableAsPropertyHelper<string> _description;
        private readonly ObservableAsPropertyHelper<string> _author;
        private readonly ObservableAsPropertyHelper<string> _company;
        private readonly ObservableAsPropertyHelper<string> _dependencies;
        private readonly ObservableAsPropertyHelper<string> _url;
        private readonly ObservableAsPropertyHelper<string> _version;
        private readonly ObservableAsPropertyHelper<string> _inferenceType;
        private readonly ObservableAsPropertyHelper<string> _operatingSystems;
        private readonly ObservableAsPropertyHelper<IObjectDetectionPlugin> _plugin;
        public PluginInfoViewModel(IObjectDetectionPlugin plugin, IPluginManager manager)
        {
            Refresh = ReactiveCommand
                .CreateFromTask<IObjectDetectionPlugin, IObjectDetectionPlugin>(
                async p => await manager.LoadPlugin(p.Tag, p.Version));
            
            _plugin = Refresh
                .Select(p => p).
                ToProperty(this, x => x.Plugin);
            
            _tag = Refresh
                .Select(p => p.Tag).
                ToProperty(this, x => x.Tag);

            _name = Refresh
                .Select(p => p.Name).
                ToProperty(this, x => x.Name);
            
            _description = Refresh
                .Select(p => p.Description).
                ToProperty(this, x => x.Description);
            
            _author = Refresh
                .Select(p => p.Author).
                ToProperty(this, x => x.Author);
            
            _company = Refresh
                .Select(p => p.Company).
                ToProperty(this, x => x.Company);
            
            _dependencies = Refresh
                .Select(p => p.GetDependenciesAsString()).
                ToProperty(this, x => x.Dependencies);
            
            _url = Refresh
                .Select(p => p.Url).
                ToProperty(this, x => x.Url);
            
            _version = Refresh
                .Select(p => p.Version.ToString()).
                ToProperty(this, x => x.Version);
            
            _inferenceType = Refresh
                .Select(p => p.InferenceType.ToString()).
                ToProperty(this, x => x.InferenceType);
            
            _operatingSystems = Refresh
                .Select(p => p.GetOperatingSystemsAsString()).
                ToProperty(this, x => x.OperatingSystems);

            _hasErrorMessage = Refresh
                .ThrownExceptions
                .Select(exception => true)
                .ToProperty(this, x => x.HasErrorMessage);

            _errorMessage = Refresh
                .ThrownExceptions
                .Select(exception => $"can not load plugin: {exception.Message}")
                .ToProperty(this, x => x.ErrorMessage);

            Refresh.Execute(plugin).Subscribe();
        }
        public ReactiveCommand<IObjectDetectionPlugin, IObjectDetectionPlugin> Refresh { get; }
        public string Tag => _tag.Value;
        public string Name => _name.Value;
        public string Description => _description.Value;
        public string Author => _author.Value;
        public string Company => _company.Value;
        public string Dependencies => _dependencies.Value;
        public string Url => _url.Value;
        public string Version => _version.Value;
        public string InferenceType => _inferenceType.Value;
        public string OperatingSystems => _operatingSystems.Value;
        public IObjectDetectionPlugin Plugin => _plugin.Value;
        public string ErrorMessage => _errorMessage.Value;
        public bool HasErrorMessage => _hasErrorMessage.Value;
    }
}