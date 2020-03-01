using static RescuerLaApp.Models.Enums;

namespace RescuerLaApp.Models
{
    public class AppStatusInfo
    {
        public Status Status { get; private set; }
        public string StringStatus { get; private set; }

        public void ChangeCurrentStatus(Status newStatus, string statusString)
        {
            if (string.IsNullOrWhiteSpace(statusString))
                statusString = newStatus.ToString();
            Status = newStatus;
            StringStatus = statusString;
        }
    }
}
