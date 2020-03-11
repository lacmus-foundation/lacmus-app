using System;
using ReactiveUI;

namespace RescuerLaApp.ViewModels
{
    public class SecondWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        
        public SecondWizardViewModel(IScreen screen) => HostScreen = screen;
    }
}