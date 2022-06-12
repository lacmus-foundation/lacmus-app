using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace LacmusApp.Image.Models
{
    [Serializable]
    [XmlRoot("annotation")]
    public class Annotation
    {
        [XmlElement("folder")] public string Folder { get; set; } = "VocGalsTfl";
        [XmlElement("filename")] public string Filename { get; set; }
        [XmlElement("source")] public Source Source { get; set; }
        [XmlElement("size")] public Size Size { get; set; }
        [XmlElement("segmented")] public int Segmented { get; set; }
        [XmlElement("object")] public List<Object> Objects { get; set; }
    }
}