using System.ComponentModel;
using System.Reactive;
using LacmusApp.Appearance.Interfaces;
using ReactiveUI;

namespace LacmusApp.Screens.Interfaces
{
    public interface IAboutViewModel : INotifyPropertyChanged
    {
        public IVersionViewModel VersionViewModel { get; }
        public ReactiveCommand<Unit, Unit> OpenLicenseCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenGithubCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenSiteCommand { get; }
    }
}