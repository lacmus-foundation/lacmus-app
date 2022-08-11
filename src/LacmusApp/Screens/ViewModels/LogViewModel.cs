using System.IO;
using LacmusApp.Screens.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace LacmusApp.Screens.ViewModels;

public class LogViewModel : ReactiveObject, ILogViewModel, ILogEventSink
{
    private readonly ITextFormatter _formatter;
    
    public LogViewModel()
    {
        LogText = "";
        var template = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
        _formatter = new MessageTemplateTextFormatter(template);
    }

    [Reactive] public string LogText { get; private set; }
    
    public void Emit(LogEvent logEvent)
    {
        var sw = new StringWriter();
        _formatter.Format(logEvent, sw);
        LogText += sw.ToString();
    }
}