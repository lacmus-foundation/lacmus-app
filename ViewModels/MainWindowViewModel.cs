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
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using MessageBox.Avalonia.Models;
using MessageBox.Avalonia.Views;
using MetadataExtractor;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RescuerLaApp.Managers;
using RescuerLaApp.Models;
using RescuerLaApp.Services.Files;
using RescuerLaApp.Services.VM;
using RescuerLaApp.Views;
using Attribute = RescuerLaApp.Models.Photo.Attribute;
using Directory = System.IO.Directory;
using Object = RescuerLaApp.Models.Object;
using Size = RescuerLaApp.Models.Size;

namespace RescuerLaApp.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private INeuroModel _model; // TODO : Make field readonly 
        private readonly ApplicationStatusManager _applicationStatusManager;
        
        private readonly Window _window;
        private List<Frame> _frames = new List<Frame>();
        SourceList<PhotoViewModel> _photos { get; set; } = new SourceList<PhotoViewModel>();
        private ReadOnlyObservableCollection<PhotoViewModel> _photoCollection;
        public ReadOnlyObservableCollection<PhotoViewModel> PhotoCollection => _photoCollection;


        public MainWindowViewModel(Window window)
        {
            _window = window;
            _photos.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _photoCollection)
                .Subscribe();
            
            _applicationStatusManager = new ApplicationStatusManager();
            ApplicationStatusViewModel = new ApplicationStatusViewModel(_applicationStatusManager);


            var canGoNext = this
                .WhenAnyValue(x => x.SelectedIndex)
                .Select(index => index < Frames.Count - 1);

            // The bound button will stay disabled, when
            // there is no more frames left.
            NextImageCommand = ReactiveCommand.Create(
                () => { SelectedIndex++; },
                canGoNext);

            var canGoBack = this
                .WhenAnyValue(x => x.SelectedIndex)
                .Select(index => index > 0);

            var canSwitchBoundBox = this
                .WhenAnyValue(x => x.BoundBoxes)
                .Select(count => BoundBoxes?.Count > 0);

            // The bound button will stay disabled, when
            // there are no frames before the current one.
            PrevImageCommand = ReactiveCommand.Create(
                () => { SelectedIndex--; },
                canGoBack);

            // Add here newer commands
            SetupCommand(CanSetup(), canSwitchBoundBox, CanAuth());
        }

        private void SetupCommand(IObservable<bool> canExecute, IObservable<bool> canSwitchBoundBox,
            IObservable<bool> canAuth)
        {
            IncreaseCanvasCommand = ReactiveCommand.Create(IncreaseCanvas);
            ShrinkCanvasCommand = ReactiveCommand.Create(ShrinkCanvas);
            PredictAllCommand = ReactiveCommand.Create(PredictAll, canExecute);
            OpenFileCommand = ReactiveCommand.Create(OpenFile, canExecute);
            SaveAllCommand = ReactiveCommand.Create(SaveAll, canExecute);
            LoadModelCommand = ReactiveCommand.Create(LoadModel, canExecute);
            UpdateModelCommand = ReactiveCommand.Create(UpdateModel, canExecute);
            ShowPedestriansCommand = ReactiveCommand.Create(ShowPedestrians, canExecute);
            ShowFavoritesCommand = ReactiveCommand.Create(ShowFavorites, canExecute);
            ImportAllCommand = ReactiveCommand.Create(ImportFromXml, canExecute);
            SaveAllImagesWithObjectsCommand = ReactiveCommand.Create(SaveAllImagesWithObjects, canExecute);
            SaveFavoritesImagesCommand = ReactiveCommand.Create(SaveFavoritesImages, canExecute);
            ShowAllMetadataCommand = ReactiveCommand.Create(ShowAllMetadata, canExecute);
            ShowGeoDataCommand = ReactiveCommand.Create(ShowGeoData, canExecute);
            AddToFavoritesCommand = ReactiveCommand.Create(AddToFavorites, canExecute);
            SwitchBoundBoxesVisibilityCommand = ReactiveCommand.Create(SwitchBoundBoxesVisibility, canSwitchBoundBox);
            HelpCommand = ReactiveCommand.Create(Help);
            AboutCommand = ReactiveCommand.Create(About);
            ExitCommand = ReactiveCommand.Create(Exit);
        }

        private IObservable<bool> CanSetup()
        {
            return _applicationStatusManager.AppStatusInfoObservable
                .Select(status => status.Status != Enums.Status.Working && status.Status != Enums.Status.Unauthenticated);
        }

        private IObservable<bool> CanAuth()
        {
            return _applicationStatusManager.AppStatusInfoObservable
                .Select(status => status.Status == Enums.Status.Unauthenticated);
        }

        public void UpdateFramesRepo()
        {
            this.WhenAnyValue(x => x.SelectedIndex)
                .Skip(1)
                .Subscribe(x =>
                {
                    if (_applicationStatusManager.AppStatusInfo.Status == Enums.Status.Ready)
                        _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready,
                            $"{Enums.Status.Ready.ToString()} | {Frames[SelectedIndex].Path}");
                    SwitchBoundBoxesVisibilityToTrue();
                    FavoritesStateString =
                        Frames[SelectedIndex].IsFavorite ? "Remove from favorites" : "Add to favorites";
                    UpdateUi();
                });
        }

        #region Public API

        [Reactive] public ApplicationStatusViewModel ApplicationStatusViewModel { get; set; }
        [Reactive] public List<BoundBox> BoundBoxes { get; set; } = new List<BoundBox>();
        // TODO: update with locales
        [Reactive] public string BoundBoxesStateString { get; set; } = "Hide bound boxes";
        [Reactive] public string FavoritesStateString { get; set; } = "Add to favorites";
        [Reactive] public double CanvasWidth { get; set; } = 500;
        [Reactive] public double CanvasHeight { get; set; } = 500;
        [Reactive] public int SelectedIndex { get; set; }
        [Reactive] public List<Frame> Frames { get; set; } = new List<Frame>();
        [Reactive] public ImageBrush ImageBrush { get; set; } = new ImageBrush {Stretch = Stretch.Uniform};
        [Reactive] public bool IsShowPedestrians { get; set; }
        [Reactive] public bool IsShowFavorites { get; set; }

        public ReactiveCommand<Unit, Unit> PredictAllCommand { get; set; }
        public ReactiveCommand<Unit, Unit> NextImageCommand { get; }
        public ReactiveCommand<Unit, Unit> PrevImageCommand { get; }
        public ReactiveCommand<Unit, Unit> ShrinkCanvasCommand { get; set; }
        public ReactiveCommand<Unit, Unit> IncreaseCanvasCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveAllCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ImportAllCommand { get; set; }
        public ReactiveCommand<Unit, Unit> LoadModelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> UpdateModelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ShowPedestriansCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ShowFavoritesCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveAllImagesWithObjectsCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveFavoritesImagesCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ShowAllMetadataCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ShowGeoDataCommand { get; set; }
        public ReactiveCommand<Unit, Unit> AddToFavoritesCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SwitchBoundBoxesVisibilityCommand { get; set; }
        public ReactiveCommand<Unit, Unit> HelpCommand { get; set; }
        public ReactiveCommand<Unit, Unit> AboutCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SignUpCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SignInCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ExitCommand { get; set; }

        #endregion

        private void ShowPedestrians()
        {
            if (IsShowPedestrians)
            {
                // fix bug when application stop if focus was set on image without object
                if (!_frames.Any(x => x.IsVisible))
                {
                    IsShowPedestrians = false;
                    if (IsShowFavorites)
                    {
                        IsShowFavorites = false;
                        ShowFavorites();
                    }

                    return;
                }

                IsShowFavorites = false;
                SelectedIndex = Frames.FindIndex(x => x.IsVisible);
                Frames = _frames.FindAll(x => x.IsVisible);
                UpdateUi();
            }
            else
            {
                Frames = new List<Frame>(_frames);
                UpdateUi();
            }
        }

        private void ShowFavorites()
        {
            if (IsShowFavorites)
            {
                //fix bug when application stop if focus was set on image without object
                if (!_frames.Any(x => x.IsFavorite))
                {
                    IsShowFavorites = false;
                    ShowPedestrians();
                    return;
                }

                IsShowPedestrians = false;
                SelectedIndex = Frames.FindIndex(x => x.IsFavorite);
                Frames = _frames.FindAll(x => x.IsFavorite);
                UpdateUi();
            }
            else
            {
                Frames = new List<Frame>(_frames);
                UpdateUi();
            }
        }

        private async void LoadModel()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");

            if (_model == null)
            {
                _model = AvaloniaLocator.Current.GetService<INeuroModel>();
            }

            await _model.Load();

            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private async void UpdateModel()
        {
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | updating model...");

            if (_model == null)
            {
                _model = AvaloniaLocator.Current.GetService<INeuroModel>();
            }

            await _model.UpdateModel();

            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "");
        }

        private async void PredictAll()
        {
            if (Frames == null || Frames.Count < 1) return;
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "Working | loading model...");

            if (_model == null)
            {
                _model = AvaloniaLocator.Current.GetService<INeuroModel>();
            }

            if (!await _model.Run())
            {
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Error, "Error: unable to load model");
                _model.Dispose();
                _model = null;
                return;
            }

            var index = 0;
            _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working,
                $"Working | processing images: {index} / {Frames.Count}");
            foreach (var frame in Frames)
            {
                index++;
                frame.Rectangles = await _model.Predict(frame.Path);
                if (index < Frames.Count)
                    _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working,
                        $"Working | processing images: {index} / {Frames.Count}");

                if (frame.Rectangles.Any())
                    frame.IsVisible = true;
            }

            _frames = new List<Frame>(Frames);
            await _model.Stop();
            SelectedIndex = 0; //Fix bug when application stopped if index > 0
            UpdateUi();
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

        private async void OpenFile()
        {
            try
            {
                await Task.Factory.StartNew(async () =>
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
                        Console.WriteLine($"INFO: loaded {_photos.Count} photos.");
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: unable to load photos.\nDetails: {ex}");
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Error,
                    $"Error | {ex.Message.Replace('\n', ' ')}");
            }
        }
        private async void ImportFromXml()
        {
            try
            {
                await Task.Factory.StartNew(async () =>
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
                        Console.WriteLine($"INFO: loaded {_photos.Count} photos.");
                    });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: unable to load photos.\nDetails: {ex}");
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Error,
                    $"Error | {ex.Message.Replace('\n', ' ')}");
            }
        }

        private async void SaveAll()
        {
            try
            {
                if (!_photoCollection.Any())
                {
                    Console.WriteLine("WARN: there is no photos to save.");
                    return;
                }
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                var writer = new PhotoVMWriter(_window);
                await writer.WriteMany(_photoCollection);
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "Success | saved");
                Console.WriteLine($"INFO: saved {_photoCollection.Count} photos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: unable to save photos.\nDetails: {ex}");
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Error,
                    $"Error | {ex.Message.Replace('\n', ' ')}");
            }
        }

        private async void SaveAllImagesWithObjects()
        {
            try
            {
                if (!_photoCollection.Any())
                {
                    Console.WriteLine("WARN: there is no photos to save.");
                    return;
                }
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                var writer = new PhotoVMWriter(_window);
                await writer.WriteMany(_photoCollection.Where(x => x.Photo.Attribute == Attribute.WithObject));
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "Success | saved");
                Console.WriteLine($"INFO: saved {_photoCollection.Count} photos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: unable to save photos.\nDetails: {ex}");
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Error,
                    $"Error | {ex.Message.Replace('\n', ' ')}");
            }
        }

        private async void SaveFavoritesImages()
        {
            try
            {
                if (!_photoCollection.Any())
                {
                    Console.WriteLine("WARN: there is no photos to save.");
                    return;
                }
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Working, "");
                var writer = new PhotoVMWriter(_window);
                await writer.WriteMany(_photoCollection.Where(x => x.Photo.Attribute == Attribute.Favorite));
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Ready, "Success | saved");
                Console.WriteLine($"INFO: saved {_photoCollection.Count} photos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: unable to save photos.\nDetails: {ex}");
                _applicationStatusManager.ChangeCurrentAppStatus(Enums.Status.Error,
                    $"Error | {ex.Message.Replace('\n', ' ')}");
            }
        }

        public void Help()
        {
            OpenUrl("https://github.com/lizaalert/lacmus/wiki");
        }

        public void ShowGeoData()
        {
            var msg = string.Empty;
            var rows = 0;
            var directories = ImageMetadataReader.ReadMetadata(Frames[SelectedIndex].Path);
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
                ContentTitle = $"Geo position of {Path.GetFileName(Frames[SelectedIndex].Path)}",
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
            var tb = new TextTableBuilder();
            tb.AddRow("Group", "Tag name", "Description");
            tb.AddRow("-----", "--------", "-----------");


            var directories = ImageMetadataReader.ReadMetadata(Frames[SelectedIndex].Path);
            foreach (var directory in directories)
            foreach (var tag in directory.Tags)
                tb.AddRow(directory.Name, tag.Name, tag.Description);

            var msgbox = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
            {
                ButtonDefinitions = ButtonEnum.Ok,
                ContentTitle = $"Metadata of {Path.GetFileName(Frames[SelectedIndex].Path)}",
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
        }

        public void AddToFavorites()
        {
            Frames[SelectedIndex].IsFavorite = !Frames[SelectedIndex].IsFavorite;

            if (IsShowFavorites)
            {
                IsShowPedestrians = false;
                //fix bug when application stop if focus was set on image without object
                if (!_frames.Any(x => x.IsFavorite))
                {
                    IsShowFavorites = false;
                    ShowFavorites();
                    return;
                }

                SelectedIndex = Frames.FindIndex(x => x.IsFavorite);
                Frames = _frames.FindAll(x => x.IsFavorite);
                UpdateUi();
            }
        }

        public void SwitchBoundBoxesVisibility()
        {
            var isVisible = true;

            if (BoundBoxes == null) return;
            if (BoundBoxes.Count > 0)
                isVisible = BoundBoxes[0].IsVisible;

            foreach (var rectangle in BoundBoxes)
            {
                rectangle.IsVisible = !isVisible;
            }

            BoundBoxesStateString = BoundBoxes[0].IsVisible ? "Hide bound boxes" : "Show bound boxes";

            UpdateUi();
        }

        private void SwitchBoundBoxesVisibilityToTrue()
        {
            if (BoundBoxes == null || BoundBoxes[0].IsVisible) return;

            foreach (var rectangle in BoundBoxes)
            {
                rectangle.IsVisible = true;
            }

            BoundBoxesStateString = "Hide bound boxes";
        }

        public async void About()
        {
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
                Console.WriteLine(e);
            }
        }

        private void UpdateUi()
        {
            /*TODO: Вынести сюда все функции обновления UI*/
            ImageBrush.Source = new Bitmap(Frames[SelectedIndex].Path); //replace to frame.load(...)
            CanvasHeight = ImageBrush.Source.PixelSize.Height;
            CanvasWidth = ImageBrush.Source.PixelSize.Width;
            if (Frames[SelectedIndex].Rectangles != null && Frames[SelectedIndex].Rectangles.Any())
            {
                BoundBoxes = new List<BoundBox>(Frames[SelectedIndex].Rectangles);
            }
            else
            {
                BoundBoxes = null;
            }
        }
    }
}