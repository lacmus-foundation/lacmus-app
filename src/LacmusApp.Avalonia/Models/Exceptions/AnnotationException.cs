using System;

namespace LacmusApp.Avalonia.Models.Exceptions
{
    public class AnnotationException : Exception
    {
        public AnnotationException(string message) : base($"FrameException: {message}") { }
    }
}