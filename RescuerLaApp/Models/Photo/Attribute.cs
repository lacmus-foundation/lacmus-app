using System;

namespace RescuerLaApp.Models.Photo
{
    [Flags]
    public enum Attribute
    {
        Empty = 1,
        NotProcessed = 2,
        WithObject = 4,
        Favorite = 8
    }
}