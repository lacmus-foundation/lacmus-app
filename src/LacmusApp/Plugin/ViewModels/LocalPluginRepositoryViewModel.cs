using System.Collections.Generic;
using System.Reactive;
using LacmusApp.Plugin.Interfaces;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.ViewModels
{
    public class LocalPluginRepositoryViewModel : ReactiveObject, ILocalPluginRepositoryViewModel
    {
        public IReadOnlyCollection<IObjectDetectionPlugin> Plugins { get; }
        public IObjectDetectionPlugin SelectedPlugin { get; set; }
        public ReactiveCommand<IObjectDetectionPlugin, Unit> ActivatePlugin { get; }
        public ReactiveCommand<IObjectDetectionPlugin, Unit> RemovePlugin { get; }
        public string ErrorMessage { get; }
        public bool HasErrorMessage { get; }
    }
}