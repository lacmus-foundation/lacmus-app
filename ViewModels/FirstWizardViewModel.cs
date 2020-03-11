using System;
using ReactiveUI;

namespace RescuerLaApp.ViewModels
{
    public class FirstWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        
        public FirstWizardViewModel(IScreen screen) => HostScreen = screen;
    }
}