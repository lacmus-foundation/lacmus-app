using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using LacmusPlugin;
using ReactiveUI;

namespace LacmusApp.Plugin.Interfaces
{
    public interface ILocalPluginRepositoryViewModel : INotifyPropertyChanged
    {
        IReadOnlyCollection<IPluginViewModel> Plugins { get; }
        ReactiveCommand<Unit, IReadOnlyCollection<IPluginViewModel>> Refresh { get; }
        ReactiveCommand<Unit, Unit> Import { get; }
        string ErrorMessage { get; }
        bool HasErrorMessage { get; }
    }
}