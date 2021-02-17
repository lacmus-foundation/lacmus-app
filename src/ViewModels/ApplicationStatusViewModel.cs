using System;
using System.Reactive.Linq;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Extensions;
using LacmusApp.Managers;
using LacmusApp.Models;

namespace LacmusApp.ViewModels
{
    public class ApplicationStatusViewModel : ReactiveObject
    {
        public ApplicationStatusViewModel(
            ApplicationStatusManager applicationStatusManager)
        {
            applicationStatusManager.AppStatusInfoObservable
                .Subscribe(UpdateStatus);
        }
        
        [Reactive] public ISolidColorBrush StatusColor { get; private set; }
        [Reactive] public string StringStatus { get; private set; }
        
        private void UpdateStatus(AppStatusInfo status)
        {
            StatusColor = status.GetColor();
            StringStatus = status.StringStatus;
        }
    }
}