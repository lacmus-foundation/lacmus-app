using System;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Avalonia.Extensions;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Avalonia.Models;

namespace LacmusApp.Avalonia.ViewModels
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