using System.ComponentModel;
using System.Reactive;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.Interfaces
{
    public interface IPluginViewModel : IObjectDetectionPlugin, INotifyPropertyChanged
    {
        ReactiveCommand<Unit, Unit> Install { get; }
        ReactiveCommand<Unit, IObjectDetectionPlugin> Activate { get; }
        ReactiveCommand<Unit, Unit> Remove { get; }
        string ErrorMessage { get; }
        bool HasErrorMessage { get; }
    }
}