using System.ComponentModel;
using System.Reactive;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.Interfaces
{
    public interface ILocalPluginViewModel : IPluginViewModel
    {
        ReactiveCommand<Unit, Unit> Remove { get; }
    }
}