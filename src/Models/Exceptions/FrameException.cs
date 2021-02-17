using System;

namespace LacmusApp.Models.Exceptions
{
    public class FrameException : Exception
    {
        public FrameException(string message) : base($"FrameException: {message}") { }
    }
}