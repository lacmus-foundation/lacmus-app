using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using LacmusApp.Image.Interfaces;
using LacmusPlugin;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LacmusApp.Image.Models
{
    public class Image<TBrush> : ReactiveObject, IImage<TBrush>
    {
        private readonly ObservableAsPropertyHelper<bool> _isHasObjects;
        private readonly ObservableAsPropertyHelper<string> _name;

        public Image()
        {
            Detections = new List<IObject>();
            _isHasObjects = this.WhenAnyValue(x => x.Detections)
                .Select(x => x.Any())
                .ToProperty(this, x => x.IsHasObjects);
            
            _name = this.WhenAnyValue(x => x.Path)
                .Select(x =>
                {
                    var name = System.IO.Path.GetFileName(Path);
                    if (name is {Length: <= 15})
                        return name;

                    if (name != null)
                    {
                        var digitName = Regex.Replace(name, @"[^\d]", "");
                        if (!string.IsNullOrWhiteSpace(digitName))
                            name = digitName;
                    }

                    if (name is { Length: > 15 })
                    {
                        name = name.Substring(0, 3) + "{~}" + name.Substring(name.Length - 10);
                    }
            
                    return name;
                })
                .ToProperty(this, x => x.Name);
        }

        public int Height { get; set; }
        public int Width { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public IEnumerable<ExifData> ExifDataCollection { get; set; }
        public TBrush Brush { get; set; }
        public string Path { get; set; } = "";
        public string Name => _name.Value;
        [Reactive] public IEnumerable<IObject> Detections { get; set; }
        public bool IsHasObjects => _isHasObjects.Value;
        [Reactive] public bool IsFavorite { get; set; }
        [Reactive] public bool IsWatched { get; set; }
    }
}