using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RescuerLaApp.Managers;
using RescuerLaApp.Models;
using Serilog;

namespace RescuerLaApp.ViewModels
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

        public WizardWindowViewModel(Window window, 
            ApplicationStatusManager manager,
            SourceList<PhotoViewModel> photos,
            int selectedIndex)
        {
            _window = window;
            _router = new RoutingState();
            _firstWizardViewModel = new FirstWizardViewModel(this);
            _secondWizardViewModel = new SecondWizardViewModel(this);
            _thirdWizardViewModel = new ThirdWizardViewModel(this, manager);
            _fourthWizardViewModel = new FourthWizardViewModel(this, manager,
                photos, selectedIndex);

            canGoNext = this
                .WhenAnyValue(x => x.CanGoNext);
            canGoBack = this.
                WhenAnyValue(x => x.CanGoBack);
            
            CanGoNext = true;
            CanGoBack = false;
            GoNext = ReactiveCommand.Create(Next, canGoNext);
            GoBack = ReactiveCommand.Create(Back, canGoBack);
            
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
                    NextButtonText = "Next";
                    BackButtonText = "Back";
                    Router.NavigateBack.Execute();
                    break;
                case 4:
                    CanGoBack = false;
                    Router.NavigationStack.Clear();
                    Router.Navigate.Execute(_firstWizardViewModel);
                    NextButtonText = "Next";
                    BackButtonText = "Back";
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
                    NextButtonText = "Next";
                    BackButtonText = "Back";
                    break;
                case 1:
                    CanGoBack = true;
                    Router.Navigate.Execute(_secondWizardViewModel);
                    NextButtonText = "Next";
                    BackButtonText = "Back";
                    break;
                case 2:
                    CanGoBack = true;
                    Router.Navigate.Execute(_thirdWizardViewModel);
                    _thirdWizardViewModel.UpdateModelStatus();
                    NextButtonText = "Predict all";
                    BackButtonText = "Back";
                    break;
                case 3:
                    Router.Navigate.Execute(_fourthWizardViewModel);
                    NextButtonText = "Finish";
                    BackButtonText = "Repeat";
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
                    NextButtonText = "Next";
                    BackButtonText = "Back";
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