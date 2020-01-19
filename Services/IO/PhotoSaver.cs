using System;
using RescuerLaApp.Models.Photo;

namespace RescuerLaApp.Services.IO
{
    public class PhotoSaver : IPhotoSaver
    {
        public void Save(Photo photo, string source)
        {
            var saver = new FileSaver();
            using (var stream = saver.Save(source))
            {
                try
                {
                    photo.ImageBrush.Source.Save(stream);
                }
                catch (Exception e)
                {
                    throw new Exception($"unable save image to {source}", e);
                }
            }
        }
    }
}