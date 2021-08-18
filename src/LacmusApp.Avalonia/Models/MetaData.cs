using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LacmusApp.Avalonia.Models
{
    public class MetaData
    {
        [Reactive] public string Group { get; set; }
        [Reactive] public string TagName { get; set; }
        [Reactive] public string Description { get; set; }

        public MetaData(string group, string tagName, string description)
        {
            Group = group;
            TagName = tagName;
            Description = description;
        }
    }
}