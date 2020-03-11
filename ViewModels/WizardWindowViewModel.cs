using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
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
        public RoutingState Router => _router;
            
        // The command that navigates a user to first view model.
        public ReactiveCommand<Unit, Unit> GoNext { get; }

        // The command that navigates a user back.
        public ReactiveCommand<Unit, Unit> GoBack => Router.NavigateBack;

        [Reactive] public string NextButtonText { get; private set; } = "Next";
        [Reactive] public string BackButtonText { get; private set; } = "Back";
        [Reactive] public string InputPath { get; set; } = string.Empty;
        [Reactive] public string OutputPath { get; set; } = string.Empty;

        public WizardWindowViewModel(Window window)
        {
            _window = window;
            _router = new RoutingState();
            _firstWizardViewModel = new FirstWizardViewModel(this);
            GoNext = ReactiveCommand.Create(
                () =>
                {
                    switch (Router.NavigationStack.Count)
                    {
                        case 0:
                            Router.Navigate.Execute(_firstWizardViewModel);
                            NextButtonText = "Next";
                            BackButtonText = "Back";
                            break;
                        case 1:
                            Router.Navigate.Execute(new SecondWizardViewModel(this));
                            NextButtonText = "Next";
                            BackButtonText = "Back";
                            break;
                        case 2:
                            Router.Navigate.Execute(new ThirdWizardViewModel(this));
                            NextButtonText = "Predict all";
                            BackButtonText = "Back";
                            break;
                        case 3:
                            Router.Navigate.Execute(new FourthWizardViewModel(this));
                            NextButtonText = "Finish";
                            BackButtonText = "Repeat";
                            break;
                    }
                }
            );
            Log.Information("Wizard started.");
        }
    }
}