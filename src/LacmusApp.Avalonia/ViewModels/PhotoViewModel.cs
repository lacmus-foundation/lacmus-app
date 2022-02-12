using System.Collections.Generic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Avalonia.Extensions;
using LacmusApp.Avalonia.Models;
using LacmusApp.Avalonia.Models.Photo;

namespace LacmusApp.Avalonia.ViewModels
{
    //TODO: add PhotoViewModelManager or something else like this
    public class PhotoViewModel : ReactiveObject
    {
        private Annotation _annotation;
        public PhotoViewModel(int id, Photo photo, Annotation annotation)
        {
            Id = id;
            Photo = photo;
            _annotation = annotation;
            UpdatePhotoInfo(Annotation);
        }
        [Reactive] public Photo Photo { get; set; }

        [Reactive] public Annotation Annotation
        {
            get => _annotation;
            set
            {
                _annotation = value;
                UpdatePhotoInfo(_annotation);
            }
        }
        [Reactive] public string Caption { get; private set; }
        [Reactive] public string Path { get; private set; }
        [Reactive] public bool IsHasObject { get; set; } = false;
        [Reactive] public bool IsFavorite { get; set; } = false;
        [Reactive] public bool IsWatched { get; set; } = false;
        [Reactive] public IEnumerable<BoundBox> BoundBoxes { get; set; } = new List<BoundBox>();
        public int Id { get; set; }

        private void UpdatePhotoInfo(Annotation annotation)
        {
            Caption = annotation.GetCaption();
            Path = annotation.GetPhotoPath();
        }
    }
}