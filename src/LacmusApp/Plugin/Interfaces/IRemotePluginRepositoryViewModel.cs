using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.Interfaces
{
    public interface IRemotePluginRepositoryViewModel : INotifyPropertyChanged
    {
        IReadOnlyCollection<IObjectDetectionPlugin> Plugins { get; }
        IObjectDetectionPlugin SelectedPlugin { get; set; }
        ReactiveCommand<IObjectDetectionPlugin, Unit> InstallPlugin { get; }
        ReactiveCommand<Unit, IReadOnlyCollection<IObjectDetectionPlugin>> Refresh { get; }
        string ErrorMessage { get; }
        bool HasErrorMessage { get; }
    }
}