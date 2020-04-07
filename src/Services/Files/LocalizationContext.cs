using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;


namespace LacmusApp.Services.Files
{
    public class LocalizationContext : ReactiveObject
    {
        [Reactive] public Language Language {get; set;}

        #region STRINGS FOR LOCALIZING
        private string _file;
        [Reactive] public string File 
        {
            get { return _file; }
            set { this.RaiseAndSetIfChanged(ref _file, value); }
        }
        private string _openDirectory;
        [Reactive] public string OpenDirectory
        {
            get { return _openDirectory; }
            set { this.RaiseAndSetIfChanged(ref _openDirectory, value); }
        }
        private string _importAllFromXml;
        [Reactive] public string ImportAllFromXml
        {
            get { return _importAllFromXml; }
            set { this.RaiseAndSetIfChanged(ref _importAllFromXml, value); }
        }
        private string _exportToXml;
        [Reactive] public string ExportToXml
        {
            get { return _exportToXml; }
            set { this.RaiseAndSetIfChanged(ref _exportToXml, value); }
        }
        private string _exportFavoritesToXml;
        [Reactive] public string ExportFavoritesToXml
        {
            get { return _exportFavoritesToXml; }
            set { this.RaiseAndSetIfChanged(ref _exportFavoritesToXml, value); }
        }
        private string _settings;
        [Reactive] public string Settings
        {
            get { return _settings; }
            set { this.RaiseAndSetIfChanged(ref _settings, value); }
        }
        private string _exit;
        [Reactive] public string Exit
        {
            get { return _exit; }
            set { this.RaiseAndSetIfChanged(ref _exit, value); }
        }
        private string _model;
        [Reactive] public string Model
        {
            get { return _model; }
            set { this.RaiseAndSetIfChanged(ref _model, value); }
        }
        private string _loadModel;
        [Reactive] public string LoadModel
        {
            get { return _loadModel; }
            set { this.RaiseAndSetIfChanged(ref _loadModel, value); }
        }
        private string _updateModel;
        [Reactive] public string UpdateModel
        {
            get { return _updateModel; }
            set { this.RaiseAndSetIfChanged(ref _updateModel, value); }
        }
        private string _image;
        [Reactive] public string Image
        {
            get { return _image; }
            set { this.RaiseAndSetIfChanged(ref _image, value); }
        }
        private string _predictThis;
        [Reactive] public string PredictThis
        {
            get { return _predictThis; }
            set { this.RaiseAndSetIfChanged(ref _predictThis, value); }
        }
        private string _predictAll;
        [Reactive] public string PredictAll
        {
            get { return _predictAll; }
            set { this.RaiseAndSetIfChanged(ref _predictAll, value); }
        }
        private string _increase;
        [Reactive] public string Increase
        {
            get { return _increase; }
            set { this.RaiseAndSetIfChanged(ref _increase, value); }
        }
        private string _shrink;
        [Reactive] public string Shrink
        {
            get { return _shrink; }
            set { this.RaiseAndSetIfChanged(ref _shrink, value); }
        }
        private string _reset;
        [Reactive] public string Reset
        {
            get { return _reset; }
            set { this.RaiseAndSetIfChanged(ref _reset, value); }
        }
        private string _next;
        [Reactive] public string Next
        {
            get { return _next; }
            set { this.RaiseAndSetIfChanged(ref _next, value); }
        }
        private string _previous;
        [Reactive] public string Previous
        {
            get { return _previous; }
            set { this.RaiseAndSetIfChanged(ref _previous, value); }
        }
        private string _help;
        [Reactive] public string Help
        {
            get { return _help; }
            set { this.RaiseAndSetIfChanged(ref _help, value); }
        }
        private string _openUserGuide;
        [Reactive] public string OpenUserGuide
        {
            get { return _openUserGuide; }
            set { this.RaiseAndSetIfChanged(ref _openUserGuide, value); }
        }
        private string _about;
        [Reactive] public string About
        {
            get { return _about; }
            set { this.RaiseAndSetIfChanged(ref _about, value); }
        }
        private string _saveAll;
        [Reactive] public string SaveAll
        {
            get { return _saveAll; }
            set { this.RaiseAndSetIfChanged(ref _saveAll, value); }
        }
        private string _showPedestrians;
        [Reactive] public string ShowPedestrians
        {
            get { return _showPedestrians; }
            set { this.RaiseAndSetIfChanged(ref _showPedestrians, value); }
        }
        private string _showFavorites;
        [Reactive] public string ShowFavorites
        {
            get { return _showFavorites; }
            set { this.RaiseAndSetIfChanged(ref _showFavorites, value); }
        }
        private string _showGeoPosition;
        [Reactive] public string ShowGeoPosition
        {
            get { return _showGeoPosition; }
            set { this.RaiseAndSetIfChanged(ref _showGeoPosition, value); }
        }
        private string _selectLanguage;
        [Reactive] public string SelectLanguage
        {
            get { return _selectLanguage; }
            set { this.RaiseAndSetIfChanged(ref _selectLanguage, value); }
        }
         private string _saveAs;
        [Reactive] public string SaveAs
        {
            get { return _saveAs; }
            set { this.RaiseAndSetIfChanged(ref _saveAs, value); }
        }
        private string _allPhotos;
        [Reactive] public string AllPhotos
        {
            get { return _allPhotos; }
            set { this.RaiseAndSetIfChanged(ref _allPhotos, value); }
        }
         private string _photosWithObject;
        [Reactive] public string PhotosWithObject
        {
            get { return _photosWithObject; }
            set { this.RaiseAndSetIfChanged(ref _photosWithObject, value); }
        }
         private string _favoritePhotos;
        [Reactive] public string FavoritePhotos
        {
            get { return _favoritePhotos; }
            set { this.RaiseAndSetIfChanged(ref _favoritePhotos, value); }
        }
         private string _wizard;
        [Reactive] public string Wizard
        {
            get { return _wizard; }
            set { this.RaiseAndSetIfChanged(ref _wizard, value); }
        }
         private string _border;
        [Reactive] public string Border
        {
            get { return _border; }
            set { this.RaiseAndSetIfChanged(ref _border, value); }
        }
        #endregion

        public LocalizationContext()
        {
            this.WhenAnyValue(vm=>vm.Language).Subscribe(_=>UpdateText());
            Language = new Language();
            Language=Language.English;
        }

        private void UpdateText()
        {
            switch (Language)
            {
                case Language.English:
                {
                    File="File";
                    OpenDirectory="Open Directory";
                    ImportAllFromXml="Import all from XML";
                    ExportToXml="Export to XML";
                    ExportFavoritesToXml="Export favorites to XML";
                    Settings="Settings";
                    Exit="Exit";
                    Model="Model";
                    LoadModel="Load model";
                    UpdateModel="Update model";
                    Image="Image";
                    PredictThis="Predict this";
                    PredictAll="Predict All";
                    Increase="Increase";
                    Shrink="Shrink";
                    Reset="Reset";
                    Next="Next";
                    Previous="Previous";
                    Help="Help";
                    OpenUserGuide="Open user guide";
                    About="About";
                    SaveAll="Save all";
                    ShowPedestrians="Show Pedestrians";
                    ShowFavorites="Show Favorites";
                    ShowGeoPosition="Show Geo Position";
                    SelectLanguage="Select language";
                    SaveAs="Save As";
                    AllPhotos="All photos";
                    PhotosWithObject="Photos with objects";
                    FavoritePhotos="Favorite photos";
                    Wizard="Wizard";
                    Border="Border";
                    break;
                }
                case Language.Russian:
                {
                    File="Файл";
                    OpenDirectory="Открыть папку";
                    ImportAllFromXml="Импортировать всё из XML";
                    ExportToXml="Экспортировать в XML";
                    ExportFavoritesToXml="Экспортировать избранное в XML";
                    Settings="Настройки";
                    Exit="Выход";
                    Model="Модель";
                    LoadModel="Загрузить модель";
                    UpdateModel="Обновить модель";
                    Image="Изображение";
                    PredictThis="Спрогнозировать текущее";
                    PredictAll="Спрогнозировать все";
                    Increase="Увеличить";
                    Shrink="Уменьшить";
                    Reset="Перезагрузить";
                    Next="Следующее";
                    Previous="Предыдущее";
                    Help="Помощь";
                    OpenUserGuide="Открыть руководство пользователя";
                    About="О программе";
                    SaveAll="Сохранить всё";
                    ShowPedestrians="Показать пешеходов";
                    ShowFavorites="Показать избранное";
                    ShowGeoPosition="Показать геопозицию";
                    SelectLanguage="Выберите язык";
                    SaveAs="Сохранить как";
                    AllPhotos="Все фото";
                    PhotosWithObject="Фото с объектами";
                    FavoritePhotos="Избранные фото";
                    Wizard="Помощник";
                    Border="Рамка";
                    break;
                }
            }
        }
    }

    public enum Language
    {
        Russian,
        English
    }
}
