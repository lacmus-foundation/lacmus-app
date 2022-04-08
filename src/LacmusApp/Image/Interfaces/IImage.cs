using System.Collections.Generic;
using System.ComponentModel;
using LacmusApp.Image.Models;
using LacmusPlugin;

namespace LacmusApp.Image.Interfaces
{
    public interface IImage<out TBrush> : INotifyPropertyChanged
    {
        public int Height { get; }
        public int Width { get; }
        public float Latitude { get; }
        public float Longitude { get; }
        public IEnumerable<ExifData> ExifDataCollection { get; }
        public TBrush Brush { get; }
        public string Path { get; }
        public IEnumerable<IObject> Detections { get; set; }
        public bool IsHasObjects { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsWatched { get; set; }
    }
}