using System;
using System.Reactive;
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
        private readonly ObservableAsPropertyHelper<IObjectDetectionPlugin> _plugin;
        public PluginInfoViewModel(IObjectDetectionPlugin plugin, IPluginManager manager)
        {
            Refresh = ReactiveCommand.CreateFromTask(
                async () => await manager.LoadPlugin(plugin));

            _tag = Refresh
                .Select(p => p.Tag).
                ToProperty(this, x => x.Tag);
            
            _plugin = Refresh
                .Select(p => p).
                ToProperty(this, x => x.Plugin);
            
            _hasErrorMessage = Refresh
                .ThrownExceptions
                .Select(exception => true)
                .ToProperty(this, x => x.HasErrorMessage);

            _errorMessage = Refresh
                .ThrownExceptions
                .Select(exception => $"can not load plugin: {exception.Message}")
                .ToProperty(this, x => x.ErrorMessage);

            Refresh.Execute().Subscribe();
        }
        public ReactiveCommand<Unit, IObjectDetectionPlugin> Refresh { get; }
        public IObjectDetectionPlugin Plugin => _plugin.Value;
        public string Tag  => _tag.Value;
        public string Name => _plugin.Value.Name;
        public string Description => _plugin.Value.Description;
        public string Author => _plugin.Value.Author;
        public string Company => _plugin.Value.Company;
        public string Dependencies => _plugin.Value.GetDependenciesAsString();
        public string Url => _plugin.Value.Url;
        public string Version => _plugin.Value.Version.ToString();
        public string InferenceType => _plugin.Value.InferenceType.ToString();
        public string OperatingSystems => _plugin.Value.GetOperatingSystemsAsString();
        public string ErrorMessage => _errorMessage.Value;
        public bool HasErrorMessage => _hasErrorMessage.Value;
    }
}