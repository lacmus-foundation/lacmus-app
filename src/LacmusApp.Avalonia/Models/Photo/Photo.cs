using System.Collections.Generic;
using Avalonia.Media;
using MetadataExtractor;
using ReactiveUI;

namespace LacmusApp.Avalonia.Models.Photo
{
    public class Photo : IPhoto
    {
        public ImageBrush ImageBrush { get; set; }
        public Attribute Attribute { get; set; } = Attribute.NotProcessed;
        public IReadOnlyList<Directory> MetaDataDirectories { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}