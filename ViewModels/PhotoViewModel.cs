using System.Collections.Generic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RescuerLaApp.Extensions;
using RescuerLaApp.Models;
using RescuerLaApp.Models.Photo;

namespace RescuerLaApp.ViewModels
{
    //TODO: add PhotoViewModelManager or something else like this
    public class PhotoViewModel : ReactiveObject
    {
        private Annotation _annotation;
        public PhotoViewModel(Photo photo, Annotation annotation)
        {
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
        [Reactive] public IEnumerable<BoundBox> BoundBoxes { get; set; }

        private void UpdatePhotoInfo(Annotation annotation)
        {
            Caption = annotation.GetCaption();
            Path = annotation.GetPhotoPath();
        }
    }
}