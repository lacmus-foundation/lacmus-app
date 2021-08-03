using System;
using System.Reactive;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LacmusApp.Managers;
using LacmusApp.Services;
using LacmusApp.Views;
using Serilog;

namespace LacmusApp.ViewModels
{
    public class WizardWindowViewModel : ReactiveObject, IScreen
    {
        private readonly Window _window;
        private readonly RoutingState _router;
        private readonly FirstWizardViewModel _firstWizardViewModel;
        private readonly SecondWizardViewModel _secondWizardViewModel;
        private readonly ThirdWizardViewModel _thirdWizardViewModel;
        private readonly FourthWizardViewModel _fourthWizardViewModel;
        private IObservable<bool> canGoBack;
        private IObservable<bool> canGoNext;
        public RoutingState Router => _router;
            
        // The command that navigates a user to first view model.
        public ReactiveCommand<Unit, Unit> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, Unit> GoBack { get; }

        [Reactive] public string NextButtonText { get; private set; } = "Next";
        [Reactive] public string BackButtonText { get; private set; } = "Back";
        [Reactive] public string InputPath { get; set; } = string.Empty;
        [Reactive] public string OutputPath { get; set; } = string.Empty;
        [Reactive] private bool CanGoNext { get; set; }
        [Reactive] private bool CanGoBack { get; set; }
        [Reactive] public LocalizationContext LocalizationContext { get; set; }

        public WizardWindowViewModel(WizardWindow window,
            ApplicationStatusManager manager,
            SourceList<PhotoViewModel> photos,
            int selectedIndex)
        {
            _window = window;
            LocalizationContext = window.LocalizationContext;
            _router = new RoutingState();
            _firstWizardViewModel = new FirstWizardViewModel(this, LocalizationContext);
            _secondWizardViewModel = new SecondWizardViewModel(this, LocalizationContext);
            _thirdWizardViewModel = new ThirdWizardViewModel(this, window, manager, LocalizationContext);
            _fourthWizardViewModel = new FourthWizardViewModel(this, manager,
                photos, selectedIndex, window.AppConfig, LocalizationContext);

            canGoNext = this
                .WhenAnyValue(x => x.CanGoNext);
            canGoBack = this.
                WhenAnyValue(x => x.CanGoBack);
            
            CanGoNext = true;
            CanGoBack = false;
            GoNext = ReactiveCommand.Create(Next, canGoNext);
            GoBack = ReactiveCommand.Create(Back, canGoBack);

            BackButtonText = LocalizationContext.WizardBackButtonText;
            NextButtonText = LocalizationContext.WizardNextButtonText;
            
            Log.Information("Wizard started.");
        }

        private void Back()
        {
            switch (Router.NavigationStack.Count)
            {
                case 0:
                    CanGoBack = false;
                    break;
                case 1:
                    CanGoBack = false;
                    break;
                case 2:
                    CanGoBack = true;
                    Router.NavigateBack.Execute();
                    CanGoBack = false;
                    break;
                case 3:
                    CanGoBack = true;
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    Router.NavigateBack.Execute();
                    break;
                case 4:
                    CanGoBack = false;
                    Router.NavigationStack.Clear();
                    Router.Navigate.Execute(_firstWizardViewModel);
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
            }
        }

        private async void Next()
        {
            CanGoNext = CanGoNextUpdate();
            CanGoBack = CanGoBackUpdate();
            if (!CanGoNext)
            {
                CanGoNext = true;
                return;
            }

            switch (Router.NavigationStack.Count)
            {
                case 0:
                    CanGoBack = false;
                    Router.Navigate.Execute(_firstWizardViewModel);
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
                case 1:
                    CanGoBack = true;
                    Router.Navigate.Execute(_secondWizardViewModel);
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
                case 2:
                    CanGoBack = true;
                    Router.Navigate.Execute(_thirdWizardViewModel);
                    _thirdWizardViewModel.UpdateModelStatus();
                    NextButtonText = LocalizationContext.WizardPredictAllButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
                case 3:
                    Router.Navigate.Execute(_fourthWizardViewModel);
                    NextButtonText = LocalizationContext.WizardFinishButtonText;
                    BackButtonText = LocalizationContext.WizardRepeatButtonText;
                    CanGoNext = false;
                    CanGoBack = false;
                    await _fourthWizardViewModel.OpenFile(_firstWizardViewModel.InputPath);
                    await _fourthWizardViewModel.PredictAll();
                    await _fourthWizardViewModel.SaveAll(_secondWizardViewModel.OutputPath);
                    CanGoNext = true;
                    CanGoBack = true;
                    break;
                case 4:
                    CanGoBack = true;
                    Router.NavigationStack.Clear();
                    _window.Close();
                    NextButtonText = LocalizationContext.WizardNextButtonText;
                    BackButtonText = LocalizationContext.WizardBackButtonText;
                    break;
            }
        }
        public bool CanGoNextUpdate()
        {
            if (_router.NavigationStack.Count == 0)
                return true;
            if (_router.NavigationStack.Count == 1 && _firstWizardViewModel.ValidationContext.IsValid)
                return true;
            if (_router.NavigationStack.Count == 2 && _secondWizardViewModel.ValidationContext.IsValid)
                return true;
            if (_router.NavigationStack.Count == 3 && _thirdWizardViewModel.Status == "Ready")
                return true;
            if (_router.NavigationStack.Count == 4 && _fourthWizardViewModel.Status == "done.")
                return true;
            return false;
        }
        public bool CanGoBackUpdate()
        {
            if (_router.NavigationStack.Count == 0)
                return false;
            if (_router.NavigationStack.Count == 1)
                return false;
            if (_router.NavigationStack.Count == 4 && _fourthWizardViewModel.Status != "done.")
                return false;
            return true;
        }
    }
}