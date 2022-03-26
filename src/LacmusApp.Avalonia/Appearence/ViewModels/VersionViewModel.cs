using LacmusApp.Appearance.Interfaces;

namespace LacmusApp.Avalonia.Appearence.ViewModels
{
    public class VersionViewModel : IVersionViewModel
    {
        public VersionViewModel()
        {
            var version = typeof(Program).Assembly.GetName().Version;
            if (version?.Revision != 0)
            {
                var revision = $"preview-{version?.Revision}";
                
                Version = $"{version?.Major}.{version?.Minor}.{version?.Build} {revision} beta";
            }
            else
            {
                Version = $"{version.Major}.{version.Minor}.{version.Build} beta";
            }
        }
        
        public string Version { get; }
    }
}