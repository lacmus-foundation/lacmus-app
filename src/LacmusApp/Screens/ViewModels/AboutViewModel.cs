using System.Reactive;
using LacmusApp.Appearance.Interfaces;
using LacmusApp.IO.Services;
using LacmusApp.Screens.Interfaces;
using ReactiveUI;

namespace LacmusApp.Screens.ViewModels
{
    public class AboutViewModel : ReactiveObject, IAboutViewModel
    {
        public AboutViewModel(IVersionViewModel versionViewModel)
        {
            var manager = new WebLinkManager();
            VersionViewModel = versionViewModel;
            OpenLicenseCommand = ReactiveCommand.Create(() =>
            {
                manager.OpenLink("https://github.com/lacmus-foundation/lacmus-app/blob/master/LICENSE");
            });
            OpenGithubCommand = ReactiveCommand.Create(() =>
            {
                manager.OpenLink("https://github.com/lacmus-foundation/");
            });
            OpenSiteCommand = ReactiveCommand.Create(() =>
            {
                manager.OpenLink("https://lacmus-foundation.github.io/");
            });
        }
        
        public IVersionViewModel VersionViewModel { get; }
        public ReactiveCommand<Unit, Unit> OpenLicenseCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenGithubCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenSiteCommand { get; }
    }
}