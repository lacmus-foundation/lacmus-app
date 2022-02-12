using System;
using Avalonia.Media;
using LacmusApp.Avalonia.Models;

namespace LacmusApp.Avalonia.Extensions
{
    public static class AppStatusInfoExtension
    {
        public static ISolidColorBrush GetColor(this AppStatusInfo appStatusInfo)
        {
            switch (appStatusInfo.Status)
            {
                case Enums.Status.Ready: return new SolidColorBrush(Color.FromRgb(0, 128, 255));
                case Enums.Status.Success: return new SolidColorBrush(Color.FromRgb(0, 135, 60));
                case Enums.Status.Working: return new SolidColorBrush(Color.FromRgb(226, 90, 0));
                case Enums.Status.Error: return new SolidColorBrush(Color.FromRgb(216, 14, 0));
                case Enums.Status.Unauthenticated: return new SolidColorBrush(Color.FromRgb(120, 0, 120));
                default: throw new Exception($"Invalid app status {appStatusInfo.Status.ToString()}");
            }
        }
    }
}