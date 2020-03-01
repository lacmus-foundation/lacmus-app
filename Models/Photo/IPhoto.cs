using System.Collections.Generic;
using Avalonia.Media;
using MetadataExtractor;

namespace RescuerLaApp.Models.Photo
{
    public interface IPhoto
    {
        ImageBrush ImageBrush { get; set; }
        Attribute Attribute { get; set; }
        IReadOnlyList<Directory> MetaDataDirectories { get; set; }
    }
}