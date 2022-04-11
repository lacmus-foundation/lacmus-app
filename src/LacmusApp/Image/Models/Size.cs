using System.Xml.Serialization;

namespace LacmusApp.Image.Models
{
    public class Size
    {
        [XmlElement("height")] public int Height { get; set; }
        [XmlElement("width")] public int Width { get; set; }
        [XmlElement("depth")] public byte Depth { get; set; } = 3;
    }
}