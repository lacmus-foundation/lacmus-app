using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace RescuerLaApp.Services
{
    public class AvaloniaFileWriter : Interfaces.IFileWriter
    {
        private readonly Window _window;

        public AvaloniaFileWriter(Window window)  => _window = window;

        public async Task<Stream> Write(string name)
        {
            var fileDialog = new OpenFolderDialog();
            var folder = await fileDialog.ShowAsync(_window);
            var path = Path.Combine(folder, name);
            return File.Create(path);
        }
    }
}