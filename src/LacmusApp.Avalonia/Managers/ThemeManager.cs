using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using LacmusApp.Appearance.Enums;
using Serilog;

namespace LacmusApp.Avalonia.Managers
{
    public class ThemeManager
    {
        private readonly StyleInclude _magmaStyle = CreateStyle("avares://Citrus.Avalonia/Magma.xaml");
        private readonly StyleInclude _candyStyle = CreateStyle("avares://Citrus.Avalonia/Candy.xaml");
        private readonly StyleInclude _citrusStyle = CreateStyle("avares://Citrus.Avalonia/Citrus.xaml");
        private readonly StyleInclude _rustStyle = CreateStyle("avares://Citrus.Avalonia/Rust.xaml");
        private readonly StyleInclude _seaStyle = CreateStyle("avares://Citrus.Avalonia/Sea.xaml");
        private readonly Window _window;

        public ThemeManager(Window window)
        {
            _window = window;
            
            // We add the style to the window styles section, so it
            // will override the default style defined in App.xaml. 
            window.Styles.Add(_citrusStyle);
        }

        public Theme CurrentTheme { get; private set; } = Theme.Light;
        
        public void UseTheme(Theme theme)
        {
            // Here, we change the first style in the main window styles
            // section, and the main window instantly refreshes. Remember
            // to invoke such methods from the UI thread.
            var style = theme switch
            {
                Theme.Light => _citrusStyle,
                Theme.Dark => _rustStyle,
                _ => throw new ArgumentOutOfRangeException(nameof(theme))
            };
            _window.Styles[0] = style;
            
            CurrentTheme = theme;
        }
        
        private static StyleInclude CreateStyle(string url)
        {
            var self = new Uri("resm:Styles?assembly=LacmusApp.Avalonia");
            return new StyleInclude(self)
            {
                Source = new Uri(url)
            };
        }
    }
}