using ReactiveUI;

namespace LacmusApp.Screens.Models
{
    public class ToDoItem : ReactiveObject
    {
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}