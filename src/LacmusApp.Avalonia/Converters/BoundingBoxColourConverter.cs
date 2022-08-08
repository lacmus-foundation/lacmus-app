using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using LacmusApp.Appearance.Enums;

namespace LacmusApp.Avalonia.Converters;

public class BoundingBoxColourConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BoundingBoxColour sourceColour
            && targetType.IsAssignableTo(typeof(IBrush)))
        {
            switch (sourceColour)
            {
                case BoundingBoxColour.Red:
                    return Brushes.Red;
                case BoundingBoxColour.Blue:
                    return Brushes.Blue;
                case BoundingBoxColour.Cyan:
                    return Brushes.Cyan;
                case BoundingBoxColour.Green:
                    return Brushes.Green;
                case BoundingBoxColour.Magenta:
                    return Brushes.Magenta;
                case BoundingBoxColour.Yellow:
                    return Brushes.Yellow;
                default:
                    return Brushes.Red;
            }
        }
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
}