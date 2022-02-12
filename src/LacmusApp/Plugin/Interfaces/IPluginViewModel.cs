using System.ComponentModel;
using System.Reactive;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.Interfaces
{
    public interface IPluginViewModel : IObjectDetectionPlugin, INotifyPropertyChanged
    {
        ReactiveCommand<Unit, IObjectDetectionPlugin> Activate { get; }
        string ErrorMessage { get; }
        bool HasErrorMessage { get; }
    }
}