using System.ComponentModel;

namespace LacmusApp.Screens.Interfaces;

public interface ILogViewModel : INotifyPropertyChanged
{
    public string LogText { get; }
}