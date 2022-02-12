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
    public class RemotePluginViewModel : ReactiveObject, IRemotePluginViewModel
    {
        private IObjectDetectionPlugin _plugin;
        private readonly ObservableAsPropertyHelper<string> _errorMessage;
        private readonly ObservableAsPropertyHelper<bool> _hasErrorMessage;
        
        public RemotePluginViewModel(IObjectDetectionPlugin plugin, IPluginManager manager)
        {
            _plugin = plugin;
            
            Install = ReactiveCommand.CreateFromTask(
                async () => await manager.InstallPlugin(plugin));

            _hasErrorMessage = Install
                .ThrownExceptions
                .Select(_ => true)
                .ToProperty(this, x => x.HasErrorMessage);

            _errorMessage = Install
                .ThrownExceptions
                .Select(_ => "Can not install plugin.")
                .ToProperty(this, x => x.ErrorMessage);
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
        public ReactiveCommand<Unit, Unit> Install { get; }
        public string ErrorMessage => _errorMessage.Value;
        public bool HasErrorMessage => _hasErrorMessage.Value;
        
        public IObjectDetectionModel LoadModel(float threshold)
        {
            throw new System.NotImplementedException();
        }
    }
}