using System;

namespace RescuerLaApp.Models.Exceptions
{
    public class AnnotationException : Exception
    {
        public AnnotationException(string message) : base($"FrameException: {message}") { }
    }
}