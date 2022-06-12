using System.Xml.Serialization;

namespace LacmusApp.Image.Models
{
    public class Object
    {
        [XmlElement("name")] public string Name { get; set; } 
        [XmlElement("pose")] public string Pose { get; set; } = "Unspecified";
        [XmlElement("truncated")] public int Truncated { get; set; }
        [XmlElement("difficult")] public int Difficult { get; set; }
        [XmlElement("bndbox")] public Box Box { get; set; }
    }
}