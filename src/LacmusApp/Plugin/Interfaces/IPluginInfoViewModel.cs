using System.ComponentModel;
using System.Reactive;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.Interfaces
{
    public interface IPluginInfoViewModel : INotifyPropertyChanged
    {
        string Tag { get; }
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Company { get; }
        string Dependencies { get; }
        string Url { get; }
        string Version { get; }
        string InferenceType { get; }
        string OperatingSystems { get; }
        ReactiveCommand<Unit, IObjectDetectionPlugin> Refresh { get; }
    }
}