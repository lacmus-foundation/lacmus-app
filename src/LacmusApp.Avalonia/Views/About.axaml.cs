using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LacmusApp.Screens.Interfaces;

namespace LacmusApp.Avalonia.Views
{
    public class About : ReactiveWindow<IAboutViewModel>
    {
        public About()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}