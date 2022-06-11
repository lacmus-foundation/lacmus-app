namespace LacmusApp.Avalonia.Models;

public struct SaveAsParams
{
    public bool SaveCrop { get; set; }
    public bool SaveXml { get; set; }
    public bool SaveImage { get; set; }
    public bool SaveDrawImage { get; set; }
    public bool SaveGeoPosition { get; set; }
}