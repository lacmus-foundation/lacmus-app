using System.ComponentModel;
using System.Reactive;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.Interfaces
{
    public interface IRemotePluginViewModel : IObjectDetectionPlugin, INotifyPropertyChanged
    {
        ReactiveCommand<Unit, Unit> Install { get; }
        string ErrorMessage { get; }
        bool HasErrorMessage { get; }
    }
}