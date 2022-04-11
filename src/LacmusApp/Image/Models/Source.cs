using System.Xml.Serialization;

namespace LacmusApp.Image.Models
{
    public class Source
    {
        [XmlElement("database")] public string DataBase { get; set; } = "Unknown";
    }
}