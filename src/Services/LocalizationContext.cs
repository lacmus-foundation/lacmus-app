using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LacmusApp.Services
{
    public class LocalizationContext : ReactiveObject
    {
        [Reactive] public Language Language {get; set;}

        #region STRINGS FOR LOCALIZING MAIN WINDOW
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
        private string _modelManager;
        [Reactive] public string ModelManager
        {
            get { return _modelManager; }
            set { this.RaiseAndSetIfChanged(ref _modelManager, value); }
        }
        private string _image;
        [Reactive] public string Image
        {
            get { return _image; }
            set { this.RaiseAndSetIfChanged(ref _image, value); }
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
        private string _favoritesStateString;
        [Reactive] public string FavoritesStateString
        {
            get { return _favoritesStateString; }
            set { this.RaiseAndSetIfChanged(ref _favoritesStateString, value); }
        }
        #endregion

        #region ABOUT WINDOW

        private string _aboutAppName;
        [Reactive] public string AboutAppName
        {
            get { return _aboutAppName; }
            set { this.RaiseAndSetIfChanged(ref _aboutAppName, value); }
        }
        
        private string _aboutVersion;
        [Reactive] public string AboutVersion
        {
            get { return _aboutVersion; }
            set { this.RaiseAndSetIfChanged(ref _aboutVersion, value); }
        }
        
        private string _aboutGintubPage;
        [Reactive] public string AboutGintubPage
        {
            get { return _aboutGintubPage; }
            set { this.RaiseAndSetIfChanged(ref _aboutGintubPage, value); }
        }
        
        private string _aboutPoweredBy;
        [Reactive] public string AboutPoweredBy
        {
            get { return _aboutPoweredBy; }
            set { this.RaiseAndSetIfChanged(ref _aboutPoweredBy, value); }
        }
        
        private string _aboutLicense;
        [Reactive] public string AboutLicense
        {
            get { return _aboutLicense; }
            set { this.RaiseAndSetIfChanged(ref _aboutLicense, value); }
        }
        
        private string _aboutLicenseButton;
        [Reactive] public string AboutLicenseButton
        {
            get { return _aboutLicenseButton; }
            set { this.RaiseAndSetIfChanged(ref _aboutLicenseButton, value); }
        }
        
        private string _aboutGinhubButton;
        [Reactive] public string AboutGinhubButton
        {
            get { return _aboutGinhubButton; }
            set { this.RaiseAndSetIfChanged(ref _aboutGinhubButton, value); }
        }
        
        private string _aboutVisitWebSiteButton;
        [Reactive] public string AboutVisitWebSiteButton
        {
            get { return _aboutVisitWebSiteButton; }
            set { this.RaiseAndSetIfChanged(ref _aboutVisitWebSiteButton, value); }
        }

        #endregion

        #region WIZARD WINDOW
        
        private string _wizardHeader;
        [Reactive] public string WizardHeader
        {
            get { return _wizardHeader; }
            set { this.RaiseAndSetIfChanged(ref _wizardHeader, value); }
        }
        
        private string _wizardDescription1;
        [Reactive] public string WizardDescription1
        {
            get { return _wizardDescription1; }
            set { this.RaiseAndSetIfChanged(ref _wizardDescription1, value); }
        }
        
        private string _wizardDescription2;
        [Reactive] public string WizardDescription2
        {
            get { return _wizardDescription2; }
            set { this.RaiseAndSetIfChanged(ref _wizardDescription2, value); }
        }
        
        private string _wizardDescription3;
        [Reactive] public string WizardDescription3
        {
            get { return _wizardDescription3; }
            set { this.RaiseAndSetIfChanged(ref _wizardDescription3, value); }
        }
        
        private string _wizardDescription4;
        [Reactive] public string WizardDescription4
        {
            get { return _wizardDescription4; }
            set { this.RaiseAndSetIfChanged(ref _wizardDescription4, value); }
        }
        
        private string _wizardDescription5;
        [Reactive] public string WizardDescription5
        {
            get { return _wizardDescription5; }
            set { this.RaiseAndSetIfChanged(ref _wizardDescription5, value); }
        }
        
        private string _wizardDescription6;
        [Reactive] public string WizardDescription6
        {
            get { return _wizardDescription6; }
            set { this.RaiseAndSetIfChanged(ref _wizardDescription6, value); }
        }
        
        private string _wizardBackButtonText;
        [Reactive] public string WizardBackButtonText
        {
            get { return _wizardBackButtonText; }
            set { this.RaiseAndSetIfChanged(ref _wizardBackButtonText, value); }
        }
        
        private string _wizardNextButtonText;
        [Reactive] public string WizardNextButtonText
        {
            get { return _wizardNextButtonText; }
            set { this.RaiseAndSetIfChanged(ref _wizardNextButtonText, value); }
        }
        
        private string _wizardPredictAllButtonText;
        [Reactive] public string WizardPredictAllButtonText
        {
            get { return _wizardPredictAllButtonText; }
            set { this.RaiseAndSetIfChanged(ref _wizardPredictAllButtonText, value); }
        }
        
        private string _wizardFinishButtonText;
        [Reactive] public string WizardFinishButtonText
        {
            get { return _wizardFinishButtonText; }
            set { this.RaiseAndSetIfChanged(ref _wizardFinishButtonText, value); }
        }
        
        private string _wizardRepeatButtonText;
        [Reactive] public string WizardRepeatButtonText
        {
            get { return _wizardRepeatButtonText; }
            set { this.RaiseAndSetIfChanged(ref _wizardRepeatButtonText, value); }
        }

        #region PAGE 4
            
        private string _wizardFourthHeader;
        [Reactive] public string WizardFourthHeader
        {
            get { return _wizardFourthHeader; }
            set { this.RaiseAndSetIfChanged(ref _wizardFourthHeader, value); }
        }
        
        private string _wizardFourthDescription;
        [Reactive] public string WizardFourthDescription
        {
            get { return _wizardFourthDescription; }
            set { this.RaiseAndSetIfChanged(ref _wizardFourthDescription, value); }
        }
        
        private string _wizardFourthTotalStatus;
        [Reactive] public string WizardFourthTotalStatus
        {
            get { return _wizardFourthTotalStatus; }
            set { this.RaiseAndSetIfChanged(ref _wizardFourthTotalStatus, value); }
        }
        
        private string _wizardFourthLoadingPhotos;
        [Reactive] public string WizardFourthLoadingPhotos
        {
            get { return _wizardFourthLoadingPhotos; }
            set { this.RaiseAndSetIfChanged(ref _wizardFourthLoadingPhotos, value); }
        }
        
        private string _wizardFourthProcessingPhotos;
        [Reactive] public string WizardFourthProcessingPhotos
        {
            get { return _wizardFourthProcessingPhotos; }
            set { this.RaiseAndSetIfChanged(ref _wizardFourthProcessingPhotos, value); }
        }
        
        private string _wizardFourthSavingResults;
        [Reactive] public string WizardFourthSavingResults
        {
            get { return _wizardFourthSavingResults; }
            set { this.RaiseAndSetIfChanged(ref _wizardFourthSavingResults, value); }
        }
        
        private string _wizardFourthStopButton;
        [Reactive] public string WizardFourthStopButton
        {
            get { return _wizardFourthStopButton; }
            set { this.RaiseAndSetIfChanged(ref _wizardFourthStopButton, value); }
        }
        
        private string _wizardFourthLogsExpander;
        [Reactive] public string WizardFourthLogsExpander
        {
            get { return _wizardFourthLogsExpander; }
            set { this.RaiseAndSetIfChanged(ref _wizardFourthLogsExpander, value); }
        }


        #endregion
        
        #region PAGE 1
            
        private string _wizardFirstHeader;
        [Reactive] public string WizardFirstHeader
        {
            get { return _wizardFirstHeader; }
            set { this.RaiseAndSetIfChanged(ref _wizardFirstHeader, value); }
        }
        
        private string _wizardFirstDescription;
        [Reactive] public string WizardFirstDescription
        {
            get { return _wizardFirstDescription; }
            set { this.RaiseAndSetIfChanged(ref _wizardFirstDescription, value); }
        }
        
        private string _wizardFirstInputWatermark;
        [Reactive] public string WizardFirstInputWatermark
        {
            get { return _wizardFirstInputWatermark; }
            set { this.RaiseAndSetIfChanged(ref _wizardFirstInputWatermark, value); }
        }
        
        private string _wizardFirstOpenPhotosButton;
        [Reactive] public string WizardFirstOpenPhotosButton
        {
            get { return _wizardFirstOpenPhotosButton; }
            set { this.RaiseAndSetIfChanged(ref _wizardFirstOpenPhotosButton, value); }
        }

        #endregion

        #region PAGE 2

        private string _wizardSecondHeader;
        [Reactive] public string WizardSecondHeader
        {
            get { return _wizardSecondHeader; }
            set { this.RaiseAndSetIfChanged(ref _wizardSecondHeader, value); }
        }
        
        private string _wizardSecondDescription1;
        [Reactive] public string WizardSecondDescription1
        {
            get { return _wizardSecondDescription1; }
            set { this.RaiseAndSetIfChanged(ref _wizardSecondDescription1, value); }
        }
        
        private string _wizardSecondDescription2;
        [Reactive] public string WizardSecondDescription2
        {
            get { return _wizardSecondDescription2; }
            set { this.RaiseAndSetIfChanged(ref _wizardSecondDescription2, value); }
        }
        
        private string _wizardSecondOutputWatermark;
        [Reactive] public string WizardSecondOutputWatermark
        {
            get { return _wizardSecondOutputWatermark; }
            set { this.RaiseAndSetIfChanged(ref _wizardSecondOutputWatermark, value); }
        }
        
        private string _wizardSecondSavePhotosButton;
        [Reactive] public string WizardSecondSavePhotosButton
        {
            get { return _wizardSecondSavePhotosButton; }
            set { this.RaiseAndSetIfChanged(ref _wizardSecondSavePhotosButton, value); }
        }

        #endregion

        #region PAGE 3
        
        private string _wizardThirdHeader;
        [Reactive] public string WizardThirdHeader
        {
            get { return _wizardThirdHeader; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdHeader, value); }
        }
        
        private string _wizardThirdDescription1;
        [Reactive] public string WizardThirdDescription1
        {
            get { return _wizardThirdDescription1; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdDescription1, value); }
        }
        
        private string _wizardThirdDescription2;
        [Reactive] public string WizardThirdDescription2
        {
            get { return _wizardThirdDescription2; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdDescription2, value); }
        }
        
        private string _wizardThirdDescription3;
        [Reactive] public string WizardThirdDescription3
        {
            get { return _wizardThirdDescription3; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdDescription3, value); }
        }
        
        private string _wizardThirdDescription4;
        [Reactive] public string WizardThirdDescription4
        {
            get { return _wizardThirdDescription4; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdDescription4, value); }
        }
        
        private string _wizardThirdModelRepository;
        [Reactive] public string WizardThirdModelRepository
        {
            get { return _wizardThirdModelRepository; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdModelRepository, value); }
        }
        
        private string _wizardThirdModelType;
        [Reactive] public string WizardThirdModelType
        {
            get { return _wizardThirdModelType; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdModelType, value); }
        }
        
        private string _wizardThirdModelVersion;
        [Reactive] public string WizardThirdModelVersion
        {
            get { return _wizardThirdModelVersion; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdModelVersion, value); }
        }
        
        private string _wizardThirdModelStatus;
        [Reactive] public string WizardThirdModelStatus
        {
            get { return _wizardThirdModelStatus; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdModelStatus, value); }
        }
        
        private string _wizardThirdModelManagerButton;
        [Reactive] public string WizardThirdModelManagerButton
        {
            get { return _wizardThirdModelManagerButton; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdModelManagerButton, value); }
        }
        
        private string _wizardThirdModelStatusUpdateButton;
        [Reactive] public string WizardThirdModelStatusUpdateButton
        {
            get { return _wizardThirdModelStatusUpdateButton; }
            set { this.RaiseAndSetIfChanged(ref _wizardThirdModelStatusUpdateButton, value); }
        }

        #endregion

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
                    //Main view model
                    //Main menu
                    //File
                    File="File";
                    OpenDirectory="Open...";
                    ImportAllFromXml="Import from XMLs...";
                    SaveAll="Save";
                    SaveAs="Save As";
                    Wizard="Wizard";
                    Settings="Settings";
                    Exit="Exit";
                    //Model
                    Model="Model";
                    LoadModel="Load model";
                    UpdateModel="Update model";
                    ModelManager="Model manager...";
                    //Image
                    Image="Image";
                    PredictAll="Predict All";
                    Increase="Increase";
                    Shrink="Shrink";
                    Reset="Reset";
                    Next="Next";
                    Previous="Previous";
                    Border="Border";
                    //Help
                    Help="Help";
                    OpenUserGuide="Open user guide";
                    About="About";
                    //ListView
                    AllPhotos="All photos";
                    PhotosWithObject="Photos with objects";
                    FavoritePhotos="Favorite photos";
                    //Context menu
                    ShowGeoPosition="Show geo position";
                    FavoritesStateString = "Add to \\ remove from favorites";
                    
                    //About Window
                    AboutAppName = "Lacmus desktop application.";
                    AboutVersion = "Version: ";
                    AboutGintubPage = "Github page: ";
                    AboutPoweredBy = "Powered by: ";
                    AboutLicense = "This program comes with ABSOLUTELY NO WARRANTY; This is free software, and you are welcome to redistribute it under GNU GPL license; Click `license` for details.";
                    AboutLicenseButton = "License";
                    AboutGinhubButton = "Github";
                    AboutVisitWebSiteButton = "Visit web site";
                    
                    //Wizard Window
                    WizardHeader = "Welcome to the photo recognition wizard!";
                    WizardDescription1 = "To process your photos with the help of the nero network you need to go through 3 simple steps:";
                    WizardDescription2 = "    Step 1: select photos to process";
                    WizardDescription3 = "    Step 2: choosing a place to save the results";
                    WizardDescription4 = "    Step 3: configure ml model";
                    WizardDescription5 = "At any time, you can close this window and continue using the program through the main interface. Thank you for using the Lacmus program.";
                    WizardDescription6 = "Click 'Next' to continue.";
                    WizardBackButtonText = "Back";
                    WizardNextButtonText = "Next";
                    WizardPredictAllButtonText = "Predict all";
                    WizardFinishButtonText = "Finish";
                    WizardRepeatButtonText = "Repeat";
                    //Page 1
                    WizardFirstHeader = "Step 1: Select input data";
                    WizardFirstDescription = "To begin, please select the input data for recognition by clicking the 'Open photos' button or by writing the full path to the input folder in the text box below.";
                    WizardFirstInputWatermark = "Enter input path here.";
                    WizardFirstOpenPhotosButton = "Open photos";
                    //Page 2
                    WizardSecondHeader = "Step 2: Select output data";
                    WizardSecondDescription1 = "Now select the output to save by clicking the 'Save photos' button or specifying the full path to the input folder in the text box below.";
                    WizardSecondDescription2 = "After processing is complete, the application will save photos and XML files with the description of recognized objects in the specified folder. You can view saved objects at any time by selecting a saved folder from the 'file - import from xml' menu.";
                    WizardSecondOutputWatermark = "Enter output path here.";
                    WizardSecondSavePhotosButton = "Save photos";
                    //Page 3
                    WizardThirdHeader = "Step 3: Setup ml model";
                    WizardThirdDescription1 = "Before proceeding, make sure that the selected configuration of the neural network model is correct.";
                    WizardThirdDescription2 = "If the configuration of the neural network model is not specified or you want to use a different configuration, go to the settings by selecting the 'file - settings' in the main menu of the program.";
                    WizardThirdDescription3 = "If your model is not loaded, configure and download it by clicking the 'Model manager' button. To download, you need to download from 2 to 6GB. Make sure your internet connection is reliable.";
                    WizardThirdDescription4 = "If everything is correct, click the 'Predict all' button to start the processing process. Upon completion of the process, the result of the program operation will appear in the selected save folder.";
                    WizardThirdModelRepository = "Ml model repository: ";
                    WizardThirdModelType = "Ml model type: ";
                    WizardThirdModelVersion = "Ml model version: ";
                    WizardThirdModelStatus = "Ml model status: ";
                    WizardThirdModelManagerButton = "Model manager";
                    WizardThirdModelStatusUpdateButton = "Refresh";
                    //Page 4
                    WizardFourthHeader = "Processing photos";
                    WizardFourthDescription = "Photo processing in progress. Please do not close the program and wait until the process ends. To force stop the processing, click 'Stop ml model' (progress will save).";
                    WizardFourthTotalStatus = "Total status: ";
                    WizardFourthLoadingPhotos = "Loading photos: ";
                    WizardFourthProcessingPhotos = "Processing photos: ";
                    WizardFourthSavingResults = "Saving results: ";
                    WizardFourthStopButton = "Stop ml model";
                    WizardFourthLogsExpander = "Details";
                    
                    //Settings
                    SelectLanguage = "Select Language";
                    break;
                }
                case Language.Russian:
                {
                    //Main view model
                    //Main menu
                    //File
                    File="Файл";
                    OpenDirectory="Открыть...";
                    ImportAllFromXml="Импортировать из XML...";
                    SaveAll="Сохранить";
                    SaveAs="Сохранить как...";
                    Wizard="Помощник";
                    Settings="Настройки";
                    Exit="Выход";
                    //Model
                    Model="Модель";
                    LoadModel="Загрузить";
                    UpdateModel="Обновить";
                    ModelManager="Менеджер моделей...";
                    //Image
                    Image="Изображение";
                    PredictAll="Обработать все";
                    Increase="Увеличеть";
                    Shrink="Уменьшить";
                    Reset="Сбросить";
                    Next="Следующее";
                    Previous="Предыдущее";
                    Border="Рамка";
                    //Help
                    Help="Помощь";
                    OpenUserGuide="Открыть руководство пользователя";
                    About="О программе";
                    //ListView
                    AllPhotos="Все фото";
                    PhotosWithObject="Фото с объектами";
                    FavoritePhotos="Избранные фото";
                    //Context menu
                    ShowGeoPosition="Показать геопозицию";
                    FavoritesStateString = "Добавить \\ удалить из избранных";
                    
                    //About Window
                    AboutAppName = "Приложение Lacmus.";
                    AboutVersion = "Версия: ";
                    AboutGintubPage = "Страница Github: ";
                    AboutPoweredBy = "При поддержке: ";
                    AboutLicense = "Данное ПО поставляется АБСОЛЮТНО БЕЗ ГАРАНТИЙ; Это свободное ПО, оно распостраняется под лиценизией GNU GPL; Нажмите `Лицензия` для просмотра.";
                    AboutLicenseButton = "Лицензия";
                    AboutGinhubButton = "Github";
                    AboutVisitWebSiteButton = "Веб-сайт";
                    
                    //Wizard Window
                    WizardHeader = "Добро пожаловать в мастер распзнования фото!";
                    WizardDescription1 = "Чтобы обработать фотографии с помощью нейронной сети (ml модели) вам необходимо выполнить 3 простых шага:";
                    WizardDescription2 = "    Шаг 1: выбирете фото для обработки";
                    WizardDescription3 = "    Шаг 2: выберете место для сохранения результатов";
                    WizardDescription4 = "    Шаг 3: выберите и настройте ml модель";
                    WizardDescription5 = "В любое время вы можете загрыть это окно и веруться к основному интерфейсу программы. Спосибо что используете Lacmus!";
                    WizardDescription6 = "Нажмите 'Далее' чтобы продолжить.";
                    WizardBackButtonText = "Назад";
                    WizardNextButtonText = "Далее";
                    WizardPredictAllButtonText = "Начать обработку";
                    WizardFinishButtonText = "Завершить";
                    WizardRepeatButtonText = "Начать сначала";
                    //Page 1
                    WizardFirstHeader = "Шаг 1: выберете входные данные";
                    WizardFirstDescription = "Чтобы начать, выберите входные данные для распознавания, нажав кнопку 'Выбрать фото' или указав полный путь к входной папке в текстовом поле ниже.";
                    WizardFirstInputWatermark = "Введите путь к входной папке сюда.";
                    WizardFirstOpenPhotosButton = "Выбрать фото";
                    //Page 2
                    WizardSecondHeader = "Шаг 2: выберете выходные данные";
                    WizardSecondDescription1 = "Теперь выберете папку для сознанения результатов, нажав на кнопку 'Сохранить фотографии' или введя полный путь к выходной папке в текстовом поле ниже.";
                    WizardSecondDescription2 = "После завершения процесса обработки приложение сохнанит результаты в выбранную папку. Вы сможете вновь открыть и просмотреть результаты в любой момент, выбрав меню 'Файл - Импортировать из XML'.";
                    WizardSecondOutputWatermark = "Введите путь к выходной папке сюда.";
                    WizardSecondSavePhotosButton = "Сохранить фотографии";
                    //Page 3
                    WizardThirdHeader = "Шаг 3: сконфигурируйте ml модель";
                    WizardThirdDescription1 = "Перед началом обработки убедитесь что вы ml модель сконфигурирована правильо и готова к использованию.";
                    WizardThirdDescription2 = "Если ml модель не готова или вы хотите выбрать другую ml модель, сконфигурируйте ее выбрав меню 'Файл - Настройки'.";
                    WizardThirdDescription3 = "Если модель не готова, вы можете сконфигурировать и загрузить ее в менеджере моделей нажав кнопку 'Менеджер ml моделей'. Помните: для загрузки ml модели из Интернета необходимо загрузить от 2 до 6 GB. Убедитесь что ваше интернет соединение надежно и что на вашем компьютере присутсвует нобходимый объем дискового пространства.";
                    WizardThirdDescription4 = "Если все хорошо и модель готова - нажмите кнопку 'Начать обработку' чтобы запустить процесс. После завершения процесса распознования результаты будут сохранены в выбранной ранее выходной папке.";
                    WizardThirdModelRepository = "Репозиторий ml модели: ";
                    WizardThirdModelType = "Тип ml модели: ";
                    WizardThirdModelVersion= "Версия ml модели: ";
                    WizardThirdModelStatus = "Готовность ml модели: ";
                    WizardThirdModelManagerButton = "Менеджер ml моделей";
                    WizardThirdModelStatusUpdateButton = "Обновить информацию";
                    //Page 4
                    WizardFourthHeader = "Обработка фотографмй";
                    WizardFourthDescription = "Выполняется процесс обработки изображений. Пожалуйста не закрывайте программу и дождитесь окончания процесса. Для принудительной остановки обработки нажчите кнопку 'Остановить ml модель' (весь прогресс будет сохранен).";
                    WizardFourthTotalStatus = "Статус выполнения: ";
                    WizardFourthLoadingPhotos = "Загрузка фотографий: ";
                    WizardFourthProcessingPhotos = "Обработка фотографий: ";
                    WizardFourthSavingResults = "Сохранение результатов: ";
                    WizardFourthStopButton = "Остановить ml модель";
                    WizardFourthLogsExpander = "Детали обработки";
                    
                    //Settings window
                    SelectLanguage = "Select Language";
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
