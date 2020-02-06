using System.Collections.Generic;
using Avalonia.Media;
using MetadataExtractor;
using ReactiveUI;

namespace RescuerLaApp.Models.Photo
{
    public class Photo : IPhoto
    {
        public ImageBrush ImageBrush { get; set; }
        public Attribute Attribute { get; set; }
        public IReadOnlyList<Directory> MetaDataDirectories { get; set; }
    }
}