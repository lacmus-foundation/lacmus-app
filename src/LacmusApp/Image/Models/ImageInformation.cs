using System.Collections.Generic;

namespace LacmusApp.Image.Models
{
    public struct ImageInformation
    {
        public float Latitude { get; init; }
        public float Longitude { get; init; }
        public IEnumerable<ExifData> ExifDataCollection { get; init; }
    }
}