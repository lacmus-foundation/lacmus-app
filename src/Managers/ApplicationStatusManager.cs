using System;
using System.Reactive.Subjects;
using LacmusApp.Models;

namespace LacmusApp.Managers
{
    public class ApplicationStatusManager
    {
        private readonly ISubject<AppStatusInfo> _appStatusInfoBehaviourSubject;
        
        public ApplicationStatusManager()
        {
            AppStatusInfo = new AppStatusInfo();
            AppStatusInfo.ChangeCurrentStatus(Enums.Status.Ready, string.Empty);
            _appStatusInfoBehaviourSubject = new BehaviorSubject<AppStatusInfo>(AppStatusInfo);
        }

        public IObservable<AppStatusInfo> AppStatusInfoObservable => _appStatusInfoBehaviourSubject;
        public AppStatusInfo AppStatusInfo { get; }

        public void ChangeCurrentAppStatus(Enums.Status status, string statusString)
        {
            AppStatusInfo.ChangeCurrentStatus(status, statusString);
            
            _appStatusInfoBehaviourSubject.OnNext(AppStatusInfo);
        }
    }
}