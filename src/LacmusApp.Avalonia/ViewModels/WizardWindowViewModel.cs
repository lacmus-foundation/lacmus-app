using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Intrinsics.X86;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Avalonia.Managers;
using LacmusApp.Avalonia.Services;
using LacmusApp.Avalonia.Views;
using LacmusApp.Screens.ViewModels;
using Serilog;

namespace LacmusApp.Avalonia.ViewModels
{
    public class WizardWindowViewModel : ReactiveObject, IScreen
    {
        private readonly Window _window;
        private readonly RoutingState _router;
        private readonly FirstWizardViewModel _firstWizardViewModel;
        private readonly SecondWizardViewModel _secondWizardViewModel;
        private readonly ThirdWizardViewModel _thirdWizardViewModel;
        private readonly FourthWizardViewModel _fourthWizardViewModel;
        public RoutingState Router => _router;
            
        // The command that navigates a user to first view model.
        public ReactiveCommand<Unit, Unit> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, Unit> GoBack { get; }

        [Reactive] public string NextButtonText { get; private set; } = "Next";
        [Reactive] public string BackButtonText { get; private set; } = "Back";
        [Reactive] public LocalizationContext LocalizationContext { get; set; }

        public WizardWindowViewModel(WizardWindow window,
            SettingsViewModel settingsViewModel,
            ApplicationStatusManager manager,
            SourceList<PhotoViewModel> photos,
            int selectedIndex)
        {
            _window = window;
            LocalizationContext = window.LocalizationContext;
            _router = new RoutingState();
            _firstWizardViewModel = new FirstWizardViewModel(this, LocalizationContext);
            _secondWizardViewModel = new SecondWizardViewModel(this, LocalizationContext);
            _thirdWizardViewModel = new ThirdWizardViewModel(this, window, settingsViewModel, manager, LocalizationContext);
            _fourthWizardViewModel = new FourthWizardViewModel(this, settingsViewModel, manager,
                photos, selectedIndex, LocalizationContext);

            var isNext = this.WhenAnyValue(
                x => x.Router.NavigationStack.Count,
                x => x._firstWizardViewModel.ValidationContext.IsValid,
                x => x._secondWizardViewModel.ValidationContext.IsValid,
                x => x._thirdWizardViewModel.Status,
                x => x._fourthWizardViewModel.Status,
                (cnt, is1, is2, is3, is4) =>
                {
                    return cnt switch
                    {
                        0 => true,
                        1 => is1,
                        2 => is2,
                        3 => is3 == "Ready",
                        4 => is4 == "done.",
                        _ => false
                    };
                });
            
            var isBack = this.WhenAnyValue(
                x => x.Router.NavigationStack.Count,
                x => x._fourthWizardViewModel.Status,
                (cnt, status) =>
                {
                    return cnt switch
                    {
                        0 => false,
                        1 => true,
                        2 => true,
                        3 => true,
                        4 => status == "done.",
                        _ => false
                    };
                });
            
            GoNext = ReactiveCommand.Create(Next, isNext);
            GoBack = ReactiveCommand.Create(Back, isBack);

            BackButtonText = LocalizationContext.WizardBackButtonText;
            NextButtonText = LocalizationContext.WizardNextButtonText;
            
            Log.Information("Wizard started.");
        }

        private void Back()
        {
            switch (Router.NavigationStack.Count)
            {
                case 0:
                    break;
                case 1:
                    Router.NavigateBack.Execute();
                    break;
                case 2:
                    Router.NavigateBack.Execute();
                    break;
                case 3:
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    Router.NavigateBack.Execute();
                    break;
                case 4:
                    Router.NavigationStack.Clear();
                    Router.Navigate.Execute(_firstWizardViewModel);
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
            }
        }

        private async void Next()
        {
            switch (Router.NavigationStack.Count)
            {
                case 0:
                    Router.Navigate.Execute(_firstWizardViewModel);
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
                case 1:
                    Router.Navigate.Execute(_secondWizardViewModel);
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
                case 2:
                    Router.Navigate.Execute(_thirdWizardViewModel);
                    _thirdWizardViewModel.UpdateModelStatus();
                    NextButtonText = LocalizationContext.WizardPredictAllButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
                case 3:
                    Router.Navigate.Execute(_fourthWizardViewModel);
                    NextButtonText = LocalizationContext.WizardFinishButtonText;
                    BackButtonText = LocalizationContext.WizardRepeatButtonText;
                    await _fourthWizardViewModel.OpenFile(_firstWizardViewModel.InputPath);
                    await _fourthWizardViewModel.PredictAll();
                    await _fourthWizardViewModel.SaveAll(_secondWizardViewModel);
                    break;
                case 4:
                    Router.NavigationStack.Clear();
                    _window.Close();
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
            }
        }
    }
}