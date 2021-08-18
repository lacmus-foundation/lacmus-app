using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LacmusApp.Avalonia.ViewModels
{
    public class FilterViewModel : ReactiveObject
    {
        [Reactive] public int FilterIndex { get; set; } = 0;
        [Reactive] public int CurrentPage { get; set; } = 0;
    }
}