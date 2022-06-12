using System.Xml.Serialization;

namespace LacmusApp.Image.Models
{
    public class Box
    {
        [XmlElement("ymin")] public int Ymin { get; set; }
        [XmlElement("xmin")] public int Xmin { get; set; }
        [XmlElement("ymax")] public int Ymax { get; set; }
        [XmlElement("xmax")] public int Xmax { get; set; }
    }
}