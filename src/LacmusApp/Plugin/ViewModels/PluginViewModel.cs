using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using LacmusApp.Plugin.Interfaces;
using LacmusPlugin;
using LacmusPlugin.Enums;
using ReactiveUI;
using OperatingSystem = LacmusPlugin.OperatingSystem;
using Version = LacmusPlugin.Version;

namespace LacmusApp.Plugin.ViewModels
{
    public class PluginViewModel : ReactiveObject, IPluginViewModel
    {
        private IObjectDetectionPlugin _plugin;
        private readonly ObservableAsPropertyHelper<string> _errorMessage;
        private readonly ObservableAsPropertyHelper<bool> _hasErrorMessage;
        
        public PluginViewModel(IObjectDetectionPlugin plugin, IPluginManager manager)
        {
            _plugin = plugin;
            Install = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    await manager.InstallPlugin(plugin);
                }
            );
            Remove = ReactiveCommand.CreateFromTask(
                async () => { await manager.UninstallPlugin(plugin); }
            );
            Activate = ReactiveCommand.CreateFromTask(
                async () => { _plugin = await manager.LoadPlugin(plugin.Tag, plugin.Version); }
            );
            
            _hasErrorMessage = Install
                .ThrownExceptions
                .Select(_ => true)
                .ToProperty(this, x => x.HasErrorMessage);

            _errorMessage = Install
                .ThrownExceptions
                .Select(_ => "Can not install plugin.")
                .ToProperty(this, x => x.ErrorMessage);
        }
        
        public IObjectDetectionModel LoadModel(float threshold)
        {
            return _plugin.LoadModel(threshold);
        }

        public string Tag => _plugin.Tag;
        public string Name => _plugin.Name;
        public string Description => _plugin.Name;
        public string Author => _plugin.Name;
        public string Company => _plugin.Company;
        public IEnumerable<string> Dependences => _plugin.Dependences;
        public string Url => _plugin.Url;
        public Version Version => _plugin.Version;
        public InferenceType InferenceType => _plugin.InferenceType;
        public HashSet<OperatingSystem> OperatingSystems => _plugin.OperatingSystems;
        public ReactiveCommand<Unit, Unit> Install { get; }
        public ReactiveCommand<Unit, Unit> Activate { get; }
        public ReactiveCommand<Unit, Unit> Remove { get; }
        public string ErrorMessage => _errorMessage.Value;
        public bool HasErrorMessage => _hasErrorMessage.Value;
    }
}