using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LacmusApp.Image.Models;
using MetadataExtractor;
using Serilog;

namespace LacmusApp.Image.Services
{
    public static class ExifConvertor
    {
        public static (IEnumerable<ExifData>, float, float, float) ConvertExif(IReadOnlyList<Directory> directories)
        {
            var list = new List<ExifData>();
            var latitude = 0f;
            var longitude = 0f;
            var altitude = 0f;
            
            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                {
                    if (tag.Name.ToLower() == "gps latitude")
                        latitude = GetGrad(tag.Description);
                    if (tag.Name.ToLower() == "gps longitude")
                        longitude = GetGrad(tag.Description);
                    if (tag.Name.ToLower() == "gps altitude")
                        altitude = GetAlt(tag.Description);
                    
                    list.Add(new ExifData()
                    {
                        Group = directory.Name,
                        Key = tag.Name,
                        Value = tag.Description
                    });
                }
            }

            return (list, latitude, longitude, altitude);
        }
        
        private static float GetGrad(string tag)
        {
            try
            {
                tag = tag.Replace('°', ';');
                tag = tag.Replace('\'', ';');
                tag = tag.Replace('"', ';');
                tag = tag.Replace(" ", "");

                var splitTag = tag.Split(';');
                var grad = float.Parse(splitTag[0]);
                var min = float.Parse(splitTag[1]);
                var sec = float.Parse(splitTag[2]);

                return grad + min / 60 + sec / 3600;
            }
            catch
            {
                Log.Warning("Unable to parse geotag");
            }

            return 0f;
        }
        
        private static float GetAlt(string tag)
        {
            try
            {
                var splitTag = tag.Split(' ').First();
                var alt = float.Parse(splitTag);
                return alt;
            }
            catch
            {
                Log.Warning("Unable to parse geotag");
            }

            return 0f;
        }
    }
}