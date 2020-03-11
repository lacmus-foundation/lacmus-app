using System;
using ReactiveUI;

namespace RescuerLaApp.ViewModels
{
    public class FourthWizardViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
        
        public FourthWizardViewModel(IScreen screen) => HostScreen = screen;
    }
}