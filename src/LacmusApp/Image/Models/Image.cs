using System.Collections.Generic;
using LacmusApp.Image.Interfaces;
using LacmusPlugin;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LacmusApp.Image.Models
{
    public class Image<TBrush> : ReactiveObject, IImage<TBrush>
    {
        public int Height { get; init; }
        public int Width { get; init; }
        public float Latitude { get; init; }
        public float Longitude { get; init; }
        public IEnumerable<ExifData> ExifDataCollection { get; init; }
        public TBrush Brush { get; init; }
        public string Path { get; init; }
        [Reactive] public IEnumerable<IObject> Detections { get; set; }
        [Reactive] public bool IsHasObjects { get; set; }
        [Reactive] public bool IsFavorite { get; set; }
        [Reactive] public bool IsWatched { get; set; }
    }
}