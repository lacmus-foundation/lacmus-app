using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using Avalonia.Collections;
using Avalonia.Controls;
using DynamicData;
using LacmusApp.Models;
using LacmusApp.Services;
using MetadataExtractor;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class MetadataViewModel : ReactiveValidationObject<MetadataViewModel>
    {
        private SourceList<MetaData> _metaDataList { get; set; } = new SourceList<MetaData>();
        private ReadOnlyObservableCollection<MetaData> _metaDataCollection;
        public ReadOnlyObservableCollection<MetaData> MetaDataCollection => _metaDataCollection;
        [Reactive] public string Latitude { get; set; } = "N/A";
        [Reactive] public string Longitude { get; set; } = "N/A";
        [Reactive] public string Altitude { get; set; } = "N/A";
        [Reactive] public LocalizationContext LocalizationContext { get; set; }
        public MetadataViewModel(Window window, IReadOnlyList<Directory> metadata, LocalizationContext localizationContext)
        {
            foreach (var directory in metadata)
            {
                foreach (var tag in directory.Tags)
                {
                    if (tag.Name.ToLower() == "gps latitude")
                        Latitude = TranslateGeoTag(tag.Description);
                    if (tag.Name.ToLower() == "gps longitude")
                        Longitude = TranslateGeoTag(tag.Description);
                    if (tag.Name.ToLower() == "gps altitude")
                        Altitude = TranslateGeoTag(tag.Description);
                    
                    _metaDataList.Add(new MetaData(directory.Name, tag.Name, tag.Description));
                }
            }

            LocalizationContext = localizationContext;
            
            _metaDataList
                .Connect()
                .Bind(out _metaDataCollection)
                .Subscribe();
            
            this.ValidationRule(
                viewModel => viewModel.Latitude,
                x => x != "N/A",
                path => $"Cannot parse gps latitude");
            this.ValidationRule(
                viewModel => viewModel.Longitude,
                x => x != "N/A",
                path => $"Cannot parse gps longitude");

            OpenYandexCommand = ReactiveCommand.Create(OpenYandex, this.IsValid());
            OpenGoogleCommand = ReactiveCommand.Create(OpenGoogle, this.IsValid());
            OpenOSMCommand = ReactiveCommand.Create(OpenOSM, this.IsValid());
        }
        public ReactiveCommand<Unit, Unit> OpenYandexCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenGoogleCommand { get; set; }
        public ReactiveCommand<Unit, Unit> OpenOSMCommand { get; set; }
        
        public void OpenYandex()
        {
            OpenUrl($"https://yandex.ru/maps/?ll={Longitude.Replace(',', '.')}%2C{Latitude.Replace(',', '.')}&z=15"+
                    $"&mode=whatshere&whatshere%5Bpoint%5D={Longitude.Replace(',', '.')}%2C{Latitude.Replace(',', '.')}&whatshere%5Bzoom%5D=15");
        }
        public void OpenGoogle()
        {
            //https://www.google.com/maps/place/"&[Latitude]&"+"&[Longitude]&"/@"&[Latitude]&","&[Longitude]&",15z
            OpenUrl($"https://www.google.ru/maps/place/{Latitude.Replace(',', '.')}+{Longitude.Replace(',', '.')}"+
                    $"/@{Latitude.Replace(',', '.')},{Longitude.Replace(',', '.')},15z");
        }
        
        public void OpenOSM()
        {
            OpenUrl($"https://www.openstreetmap.org/?mlat={Latitude.Replace(',', '.')}&mlon={Longitude.Replace(',', '.')}"+
                    $"#map=15/{Latitude.Replace(',', '.')}/{Longitude.Replace(',', '.')}");
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
                return "N/A";
            }
        }
        private void OpenUrl(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("x-www-browser", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                    throw new Exception();
            }
            catch (Exception e)
            {
                Log.Error(e,$"Unable to ope url {url}.");
            }
        }
    }
}