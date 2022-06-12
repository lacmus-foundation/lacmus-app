using ReactiveUI.Fody.Helpers;

namespace LacmusApp.Avalonia.Models;

public struct SaveAsParams
{
    [Reactive] public bool SaveCrop { get; set; }
    [Reactive] public bool SaveXml { get; set; }
    [Reactive] public bool SaveImage { get; set; }
    [Reactive] public bool SaveDrawImage { get; set; }
    [Reactive] public bool SaveGeoPosition { get; set; }
}