using System;
using ReactiveUI;

namespace RescuerLaApp.ViewModels
{
    public class ThirdWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        
        public ThirdWizardViewModel(IScreen screen) => HostScreen = screen;
    }
}