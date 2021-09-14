using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.Interfaces
{
    public interface ILocalPluginRepositoryViewModel : INotifyPropertyChanged
    {
        IReadOnlyCollection<IObjectDetectionPlugin> Plugins { get; }
        ReactiveCommand<IObjectDetectionPlugin, Unit> ActivatePlugin { get; }
        ReactiveCommand<IObjectDetectionPlugin, Unit> RemovePlugin { get; }
        string ErrorMessage { get; }
        bool HasErrorMessage { get; }
    }
}