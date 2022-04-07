using System.Collections.Generic;
using LacmusApp.Image.Interfaces;
using LacmusPlugin;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LacmusApp.Image.Models
{
    public class Image<TBrush> : ReactiveObject, IImage<TBrush>
    {
        public int Height { get; }
        public int Width { get; }
        public float Latitude { get; }
        public float Longitude { get; }
        public int Altitude { get; }
        public IEnumerable<ExifData> ExifDataCollection { get; }
        public TBrush Brush { get; }
        public string Path { get; }
        [Reactive] public IEnumerable<IObject> Detections { get; set; }
        [Reactive] public bool IsHasObjects { get; set; }
        [Reactive] public bool IsFavorite { get; set; }
        [Reactive] public bool IsWatched { get; set; }
    }
}