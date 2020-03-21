using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Logging.Serilog;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Models;
using MessageBox.Avalonia.Views;
using MetadataExtractor;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RescuerLaApp.Extensions;
using RescuerLaApp.Managers;
using RescuerLaApp.Models;
using RescuerLaApp.Models.Docker;
using RescuerLaApp.Models.ML;
using RescuerLaApp.Models.Photo;
using RescuerLaApp.Services.Files;
using RescuerLaApp.Services.IO;
using RescuerLaApp.Services.VM;
using RescuerLaApp.Views;
using Serilog;
using Serilog.Filters;
using Splat;
using Attribute = RescuerLaApp.Models.Photo.Attribute;
using Directory = System.IO.Directory;
using Object = RescuerLaApp.Models.Object;
using Size = RescuerLaApp.Models.Size;

namespace RescuerLaApp.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly ApplicationStatusManager _applicationStatusManager;
        private readonly Window _window;
        private readonly string _mlConfigPath = Path.Join("conf", "mlConfig.json");
        private int itemPerPage = 500;
        private int itemcount;
        private int _totalPages;
        SourceList<PhotoViewModel> _photos { get; set; } = new SourceList<PhotoViewModel>();
        private ReadOnlyObservableCollection<PhotoViewModel> _photoCollection;
        
        public MainWindowViewModel(Window window)
        {
            _window = window;

            var pageFilter = this
                .WhenValueChanged(x => x.CurrentPage)
                .Select(PageFilter);
            var typeFilter = this
                .WhenValueChanged(x => x.FilterIndex)
                .Select(TypeFilter);
            
            _photos.Connect()
                .Filter(pageFilter)
                .Filter(typeFilter)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _photoCollection)
                .DisposeMany()
                .Subscribe();
            
            _applicationStatusManager = new ApplicationStatusManager();
            ApplicationStatusViewModel = new ApplicationStatusViewModel(_applicationStatusManager);


            var canGoNext = this
                .WhenAnyValue(x => x.SelectedIndex)
                .Select(index => index < _photos.Count - 1);

            // The bound button will stay disabled, when
            // there is no more frames left.
            NextImageCommand = ReactiveCommand.Create(
                () => { SelectedIndex++; },
                canGoNext);

            var canGoBack = this
                .WhenAnyValue(x => x.SelectedIndex)
                .Select(index => index > 0);

            // The bound button will stay disabled, when
            // there are no frames before the current one.
            PrevImageCommand = ReactiveCommand.Create(
                () => { SelectedIndex--; },
                canGoBack);
            
            var canSwitchBoundBox = this
                .WhenAnyValue(x => x.PhotoViewModel.Photo)
                .Select(count => PhotoViewModel.Photo?.Attribute == Attribute.WithObject);
            
            // Update UI when the index changes
            // TODO: Make photo update without index
            this.WhenAnyValue(x => x.SelectedIndex)
                .Skip(1)
                .Subscribe(async x =>
                {
                    await UpdateUi();
                });

            // Add here newer commands
            SetupCommand(CanSetup(), canSwitchBoundBox);
            
            Log.Information("Application started.");
        }

        private void SetupCommand(IObservable<bool> canExecute, IObservable<bool> canSwitchBoundBox)
        {
            IncreaseCanvasCommand = ReactiveCommand.Create(IncreaseCanvas);
            ShrinkCanvasCommand = ReactiveCommand.Create(ShrinkCanvas);
            ResetCanvasCommand = ReactiveCommand.Create(ResetCanvas);
            PredictAllCommand = ReactiveCommand.Create(PredictAll, canExecute);
            OpenFileCommand = ReactiveCommand.Create(OpenFile, canExecute);
            SaveAllCommand = ReactiveCommand.Create(SaveAll, canExecute);
            LoadModelCommand = ReactiveCommand.Create(LoadModel, canExecute);
            UpdateModelCommand = ReactiveCommand.Create(UpdateModel, canExecute);
            
            ShowPedestriansCommand = ReactiveCommand.Create(ShowPedestrians, canExecute);
            ShowFavoritesCommand = ReactiveCommand.Create(ShowFavorites, canExecute);
            NextPageCommand = ReactiveCommand.Create(ShowNextPage);
            PreviousPageCommand = ReactiveCommand.Create(ShowPreviousPage);
            FirstPageCommand = ReactiveCommand.Create(ShowFirstPage);
            LastPageCommand = ReactiveCommand.Create(ShowLastPage);
            
            ImportAllCommand = ReactiveCommand.Create(ImportFromXml, canExecute);
            SaveAllImagesWithObjectsCommand = ReactiveCommand.Create(SaveAllImagesWithObjects, canExecute);
            SaveFavoritesImagesCommand = ReactiveCommand.Create(SaveFavoritesImages, canExecute);
            ShowAllMetadataCommand = ReactiveCommand.Create(ShowAllMetadata, canExecute);
            ShowGeoDataCommand = ReactiveCommand.Create(ShowGeoData, canExecute);
            AddToFavoritesCommand = ReactiveCommand.Create(AddToFavorites, canExecute);
            HelpCommand = ReactiveCommand.Create(Help);
            AboutCommand = ReactiveCommand.Create(About);
            OpenWizardCommand = ReactiveCommand.Create(OpenWizard);
            ExitCommand = ReactiveCommand.Create(Exit);
        }

        private IObservable<bool> CanSetup()
        {
            return _applicationStatusManager.AppStatusInfoObservable
                .Select(status => status.Status != Enums.Status.Working && status.Status != Enums.Status.Unauthenticated);
        }

        #region Public API

        public ReadOnlyObservableCollection<PhotoViewModel> PhotoCollection => _photoCollection;
        [Reactive] public int SelectedIndex { get; set; }
        [Reactive] public int CurrentPage { get; set; } = 0;
        
        [Reactive] public int FilterIndex { get; set; } = 0;
        [Reactive] public PhotoViewModel PhotoViewModel { get; set; }
        [Reactive] public ApplicationStatusViewModel ApplicationStatusViewModel { get; set; }
        [Reactive] public int TotalPages
        {
            get => _totalPages;
            set => _totalPages = value;
        }
        // TODO: update with locales
        [Reactive] public string FavoritesStateString { get; set; } = "Add to favorites";
        [Reactive] public double CanvasWidth { get; set; } = 500;
        [Reactive] public double CanvasHeight { get; set; } = 500;
        [Reactive] public bool IsShowPedestrians { get; set; }
        [Reactive] public bool IsShowFavorites { get; set; }

        public ReactiveCommand<Unit, Unit> PredictAllCommand { get; set; }
        public ReactiveCommand<Unit, Unit> NextImageCommand { get; }
        public ReactiveCommand<Unit, Unit> PrevImageCommand { get; }
        public ReactiveCommand<Unit, Unit> ShrinkCanvasCommand { get; set; }
        public ReactiveCommand<Unit, Unit> IncreaseCanvasCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ResetCanvasCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveAllCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ImportAllCommand { get; set; }
        public ReactiveCommand<Unit, Unit> LoadModelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> UpdateModelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ShowPedestriansCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ShowFavoritesCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveAllImagesWithObjectsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveFavoritesImagesCommand { get; set; }
        public ReactiveCommand<Unit, Unit> FirstPageCommand { get; set; }
        public ReactiveCommand<Unit, Unit> PreviousPageCommand { get; set; }
        public ReactiveCommand<Unit, Unit> NextPageCommand { get; set; }
        public ReactiveCommand<Unit, Unit> LastPageCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ShowAllMetadataCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ShowGeoDataCommand { get; set; }
        public ReactiveCommand<Unit, Unit> AddToFavoritesCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SwitchBoundBoxesVisibilityCommand { get; set; }
        public ReactiveCommand<Unit, Unit> HelpCommand { get; set; }
        public ReactiveCommand<Unit, Unit> AboutCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenWizardCommand { get; set; }

        #endregion

        private void ShowPedestrians()
        {
            if (IsShowPedestrians)
            {
            
            }
            else
            {
                
            }
        }

        private void ShowFavorites()
        {
            if (IsShowFavorites)
            {
                
            }
            else
            {
                
            }
        }
        
        private void ShowNextPage()
        {
            if (CurrentPage < TotalPages - 1)
            {
                SelectedIndex = 0;
                CurrentPage++;
            }
        }

        private void ShowPreviousPage()
        {
            if (CurrentPage > 0)
            {
                SelectedIndex = 0;
                CurrentPage--;
            }
        }

        private void ShowFirstPage()
        {
            SelectedIndex = 0;
            CurrentPage = 0;
        }

        private void ShowLastPage()
        {
            if (TotalPages > 0)
            {
                SelectedIndex = 0;
                CurrentPage = TotalPages - 1;
            }
        }
        
        private Func<PhotoViewModel, bool> PageFilter(int currentPage)
        {
           return x =>
                x.Id >= itemPerPage * currentPage && x.Id < itemPerPage * (currentPage + 1);
        }
        private Func<PhotoViewModel, bool> TypeFilter(int fitlerType)
        {
            SelectedIndex = 0;
            switch (fitlerType)
            {
                case 0:
                    return x => true;
                case 1:
                    return x => x.Photo.Attribute == Attribute.WithObject;
                case 2:
                    return x => x.Photo.Attribute == Attribute.Favorite;
                default:
                    return x => true;
            }
        }
        
        private void CalculateTotalPages()
        {
            itemcount = _photos.Count;
            if (itemcount % itemPerPage == 0)
            {
                TotalPages = (itemcount / itemPerPage);
            }
            else
            {
                TotalPages = (itemcount / itemPerPage) + 1;
            }
        }

        private async void LoadModel()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");
            //get the last version of ml model with specific config
            try
            {
                Log.Information("Loading ml model.");
                if (!File.Exists(_mlConfigPath))
                {
                    throw new Exception("There are no ml model config file. Please configure your model.");
                }
                var config = await MLModelConfigExtension.Load(_mlConfigPath);
                // get local versions
                var localVersions = await MLModel.GetInstalledVersions(config);
                if (localVersions.Any())
                {
                    config.ModelVersion = localVersions.Max();
                    Log.Information($"Find local version: {config.Image.Name}:{config.Image.Tag}.");
                }
                else
                {
                    // if there are no local models try to download it from docker registry
                    var netVersions = await MLModel.GetAvailableVersionsFromRegistry(config);
                    if (netVersions.Any())
                    {
                        config.ModelVersion = netVersions.Max();
                        Log.Information($"Find version in registry: {config.Image.Name}:{config.Image.Tag}.");
                    }
                    else
                    {
                        throw new Exception($"There are no ml models to init: {config.Image.Name}:{config.Image.Tag}");
                    }
                }
                await config.Save(_mlConfigPath);
                // init local model or download and init it from docker registry
                using(var model = new MLModel(config))
                    await model.Download();
                Log.Information("Successfully loads ml model.");
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to load model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private async void UpdateModel()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | updating model...");
            try
            {
                Log.Information("Updating ml model.");
                var oldConfig = await MLModelConfigExtension.Load(_mlConfigPath);
                var localVersions = await MLModel.GetInstalledVersions(oldConfig);
                if (localVersions.Any())
                {
                    oldConfig.ModelVersion = localVersions.Max();
                }
                else
                {
                    throw new Exception($"Nothing ml models saved: {oldConfig.Image.Name}.");
                }
                var newConfig = oldConfig;
                var netVersions = await MLModel.GetAvailableVersionsFromRegistry(newConfig);
                if (netVersions.Any())
                    newConfig.ModelVersion = netVersions.Max();
                else
                {
                    throw new Exception($"Nothing ml models in registry: {oldConfig.Image.Name}.");
                }
                if (newConfig.ModelVersion > oldConfig.ModelVersion)
                {
                    Log.Information($"Find never version of ml model {oldConfig.Image.Name}: ver {newConfig.ModelVersion} > ver {oldConfig.ModelVersion}");
                    using(var newModel = new MLModel(newConfig))
                        await newModel.Download();
                    using(var oldModel = new MLModel(oldConfig))
                        await oldModel.Remove();
                    await newConfig.Save(_mlConfigPath);
                    Log.Information("Successfully updates ml model.");
                }
                else
                {
                    Log.Information($"Ml model {oldConfig.Image.Name} is up to date: ver {newConfig.ModelVersion}.");
                }
            }
            catch (Exception e)
            {
                Log.Error(e,"Unable to update ml model.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        /// <summary>
        /// 
        /// </summary>
        private async void PredictAll()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
            try
            {
                //load config
                var config = await MLModelConfigExtension.Load(_mlConfigPath);
                using (var model = new MLModel(config))
                {
                    await model.Init();
                    var count = 0;
                    var objectCount = 0;
                    foreach (var photoViewModel in _photos.Items)
                    {
                        try
                        {
                            photoViewModel.Annotation.Objects = await model.Predict(photoViewModel);
                            photoViewModel.BoundBoxes = photoViewModel.GetBoundingBoxes();
                            if (photoViewModel.BoundBoxes.Any())
                                photoViewModel.Photo.Attribute = Attribute.WithObject;
                            objectCount += photoViewModel.BoundBoxes.Count();
                            count++;
                            Console.WriteLine($"\tProgress: {(double) count / _photos.Items.Count() * 100} %");
                        }
                        catch (Exception e)
                        {
                            Log.Error(e,$"Unable to process file {photoViewModel.Path}. Slipped.");
                        }
                    }
                    await model.Stop();
                    Log.Information($"Successfully predict {_photos.Items.Count()} photos. Find {objectCount} objects.");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to get prediction.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private void ShrinkCanvas()
        {
            Zoomer.Zoom(0.8);
        }

        private void IncreaseCanvas()
        {
            Zoomer.Zoom(1.2);
        }
        
        private void ResetCanvas()
        {
            Zoomer.Reset();
        }

        private async void OpenFile()
        {
            try
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                    var reader = new PhotoVMReader(_window);
                    _photos.Clear();
                    var photos = await reader.ReadAllFromDirByPhoto();
                    if(photos.Any())
                        _photos.AddRange(photos);
                    SelectedIndex = 0;
                    _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
                    CalculateTotalPages();
                    Log.Information($"Loads {_photos.Count} photos.");
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Unable to load photos.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }
        private async void ImportFromXml()
        {
            try
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                    var reader = new PhotoVMReader(_window);
                    _photos.Clear();
                    var photos = await reader.ReadAllFromDirByAnnotation();
                    if(photos.Any())
                        _photos.AddRange(photos);
                    SelectedIndex = 0;
                    _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
                    CalculateTotalPages();
                    Log.Information($"Loads {_photos.Count} photos.");
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Unable to load photos.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private async void SaveAll()
        {
            try
            {
                if (!_photos.Items.Any())
                {
                    Log.Warning("There are no photos to save.");
                    return;
                }
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                var writer = new PhotoVMWriter(_window);
                await writer.WriteMany(_photos.Items);
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "Success | saved");
                Log.Information($"Saved {_photos.Items.Count()} photos.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to save photos.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private async void SaveAllImagesWithObjects()
        {
            try
            {
                if (!_photos.Items.Any())
                {
                    Log.Warning("There are no photos to save.");
                    return;
                }
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                var writer = new PhotoVMWriter(_window);
                await writer.WriteMany(_photos.Items.Where(x => x.Photo.Attribute == Attribute.WithObject));
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "Success | saved");
                Log.Information($"Saved {_photos.Items.Count()} photos.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to save photos.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private async void SaveFavoritesImages()
        {
            try
            {
                if (!_photos.Items.Any())
                {
                    Log.Warning("There are no photos to save.");
                    return;
                }
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                var writer = new PhotoVMWriter(_window);
                await writer.WriteMany(_photos.Items.Where(x => x.Photo.Attribute == Attribute.Favorite));
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "Success | saved");
                Log.Information($"Saved {_photos.Items.Count()} photos.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to save photos.");
            }
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        public void OpenWizard()
        {
            Locator.CurrentMutable.Register(() => new FirstWizardView(), typeof(IViewFor<FirstWizardViewModel>));
            Locator.CurrentMutable.Register(() => new SecondWizardView(), typeof(IViewFor<SecondWizardViewModel>));
            Locator.CurrentMutable.Register(() => new ThirdWizardView(), typeof(IViewFor<ThirdWizardViewModel>));
            Locator.CurrentMutable.Register(() => new FourthWizardView(), typeof(IViewFor<FourthWizardViewModel>));
            var window = new WizardWindow();
            var context = new WizardWindowViewModel(window, _applicationStatusManager, _photos, SelectedIndex);
            window.DataContext = context;
            window.Show();
            Log.Debug("Open Wizard");
        }

        public void Help()
        {
            /*
            OpenUrl("https://github.com/lizaalert/lacmus/wiki");
            */
        }

        public void ShowGeoData()
        {
            /*
            var msg = string.Empty;
            var rows = 0;
            var directories = PhotoViewModel.Photo.MetaDataDirectories;
            foreach (var directory in directories)
            foreach (var tag in directory.Tags)
            {
                if (directory.Name.ToLower() != "gps") continue;
                if (tag.Name.ToLower() != "gps latitude" && tag.Name.ToLower() != "gps longitude" &&
                    tag.Name.ToLower() != "gps altitude") continue;

                rows++;
                msg += $"{tag.Name}: {TranslateGeoTag(tag.Description)}\n";
            }

            if (rows != 3)
                msg = "This image have hot geo tags.\nUse `Show all metadata` more for more details.";
            var msgbox = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ButtonDefinitions = ButtonEnum.Ok,
                ContentTitle = $"Geo position of {PhotoViewModel.Annotation.Filename}",
                ContentMessage = msg,
                Icon = Icon.Info,
                Style = Style.None,
                ShowInCenter = true,
                Window = new MsBoxStandardWindow
                {
                    Height = 300,
                    Width = 500,
                    CanResize = true
                }
            });
            msgbox.Show();
            */
        }

        private string TranslateGeoTag(string tag)
        {
            try
            {
                if (!tag.Contains('°'))
                    return tag;
                tag = tag.Replace('°', ';');
                tag = tag.Replace('\'', ';');
                tag = tag.Replace('"', ';');
                tag = tag.Replace(" ", "");

                var splitTag = tag.Split(';');
                var grad = float.Parse(splitTag[0]);
                var min = float.Parse(splitTag[1]);
                var sec = float.Parse(splitTag[2]);

                var result = grad + min / 60 + sec / 3600;
                return $"{result}";
            }
            catch
            {
                return tag;
            }
        }

        public void ShowAllMetadata()
        {
            /*
            var tb = new TextTableBuilder();
            tb.AddRow("Group", "Tag name", "Description");
            tb.AddRow("-----", "--------", "-----------");


            var directories = PhotoViewModel.Photo.MetaDataDirectories;
            foreach (var directory in directories)
            foreach (var tag in directory.Tags)
                tb.AddRow(directory.Name, tag.Name, tag.Description);

            var msgbox = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ButtonDefinitions = ButtonEnum.Ok,
                ContentTitle = $"Metadata of {PhotoViewModel.Annotation.Filename}",
                ContentMessage = tb.Output(),
                Icon = Icon.Info,
                Style = Style.None,
                ShowInCenter = true,
                Window = new MsBoxStandardWindow
                {
                    Height = 600,
                    Width = 1300,
                    CanResize = true
                }
            });
            msgbox.Show();
            */
        }

        public async void AddToFavorites()
        {
            if (_photoCollection[SelectedIndex].Photo.Attribute != Attribute.Favorite)
            {
                _photoCollection[SelectedIndex].Photo.Attribute = Attribute.Favorite;
            }
            else
            {
                if(_photoCollection[SelectedIndex].BoundBoxes.Any())
                    _photoCollection[SelectedIndex].Photo.Attribute = Attribute.WithObject;
                else
                {
                    _photoCollection[SelectedIndex].Photo.Attribute = Attribute.Empty;
                }
            }
            await UpdateUi();
        }

        public async void About()
        {
            /*
            var message =
                "Copyright (c) 2019 Georgy Perevozghikov <gosha20777@live.ru>\nGithub page: https://github.com/lizaalert/lacmus/. Press `Github` button for more details.\nProvided by Yandex Cloud: https://cloud.yandex.com/." +
                "\nThis program comes with ABSOLUTELY NO WARRANTY." +
                "\nThis is free software, and you are welcome to redistribute it under GNU GPLv3 conditions.\nPress `License` button to learn more about the license";

            var msgBoxCustomParams = new MessageBoxCustomParams
            {
                ButtonDefinitions = new[]
                {
                    new ButtonDefinition {Name = "Ok", Type = ButtonType.Colored},
                    new ButtonDefinition {Name = "License"},
                    new ButtonDefinition {Name = "Github"}
                },
                ContentTitle = "About",
                ContentHeader = "Lacmus desktop application. Version 0.3.3 alpha.",
                ContentMessage = message,
                Icon = Icon.Avalonia,
                Style = Style.None,
                ShowInCenter = true,
                Window = new MsBoxCustomWindow
                {
                    Height = 400,
                    Width = 1000,
                    CanResize = true
                }
            };
            var msgbox = MessageBoxManager.GetMessageBoxCustomWindow(msgBoxCustomParams);
            var result = await msgbox.Show();
            switch (result.ToLower())
            {
                case "ok": return;
                case "license":
                    OpenUrl("https://github.com/lizaalert/lacmus/blob/master/LICENSE");
                    break;
                case "github":
                    OpenUrl("https://github.com/lizaalert/lacmus");
                    break;
            }
            */
        }

        public async void Exit()
        {
            var window = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ContentTitle = "Exit",
                ContentMessage = "Do you really want to exit?",
                Icon = Icon.Info,
                Style = Style.None,
                ShowInCenter = true,
                ButtonDefinitions = ButtonEnum.YesNo
            });
            var result = await window.Show();
            if (result == ButtonResult.Yes)
                _window.Close();
        }

        private void OpenUrl(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("x-www-browser", url);
                }
            }
            catch (Exception e)
            {
                Log.Error(e,$"Unable to ope url {url}.");
            }
        }
        
        private async Task UpdateUi()
        {
            try
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    PhotoViewModel = null;
                    var currentMiniaturePhotoViewModel = PhotoCollection[SelectedIndex];
                    var photoLoader = new PhotoLoader();
                    var fullPhoto = photoLoader.Load(currentMiniaturePhotoViewModel.Path, PhotoLoadType.Full);
                    var annotation = currentMiniaturePhotoViewModel.Annotation;
                    var id = currentMiniaturePhotoViewModel.Id;
                    PhotoViewModel = new PhotoViewModel(id, fullPhoto, annotation);
                    PhotoViewModel.BoundBoxes = PhotoCollection[SelectedIndex].BoundBoxes;
                
                    CanvasHeight = PhotoViewModel.Photo.ImageBrush.Source.PixelSize.Height;
                    CanvasWidth = PhotoViewModel.Photo.ImageBrush.Source.PixelSize.Width;
                
                    if (_applicationStatusManager.AppStatusInfo.Status == Enums.Status.Ready)
                        _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready,
                            $"{Enums.Status.Ready.ToString()} | {PhotoViewModel.Path}");

                    FavoritesStateString = PhotoCollection[SelectedIndex].Photo.Attribute == Attribute.Favorite ? "Remove from favorites" : "Add to favorites";
                    
                    Log.Debug($"Ui updated to index {SelectedIndex}");
                });
            }
            catch (Exception ex)
            {
                //Log.Error(ex, "Unable to update ui.");
            }
        }
    }
}