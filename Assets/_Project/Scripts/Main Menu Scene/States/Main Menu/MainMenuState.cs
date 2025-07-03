using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MainMenuState : HFSMState, ISignalHandler<ToLoadMenuSignal>, ISignalHandler<ToGameplaySignal>,
    ISignalHandler<ToSettingsSignal>
{
    private MainMenuViewModel _viewModel;
    private MainMenuView _view;

    private GameSettings _gameSettings;

    private InputDispatcher _inputDispatcher;

    public MainMenuState(MainMenuViewModel viewModel, MainMenuView view, GameSettings gameSettings,
        InputDispatcher inputDispatcher) : base(null, view)
    {
        _viewModel = viewModel;
        _view = view;

        _inputDispatcher = inputDispatcher;

        _gameSettings = gameSettings;

        _view.Setup(_viewModel);
    }

    public override void Enter(StateEnterData enterData)
    {
        _viewModel.StartGameRequested += StartGameRequested;
        _viewModel.LoadMenuRequested += LoadMenuRequested;
        _viewModel.SettingsRequested += SettingsRequested;
        _viewModel.GameExitRequested += GameExitRequested;

        _view.Show();
    }

    public override void Exit()
    {
        _viewModel.StartGameRequested -= StartGameRequested;
        _viewModel.LoadMenuRequested -= LoadMenuRequested;
        _viewModel.SettingsRequested -= SettingsRequested;
        _viewModel.GameExitRequested -= GameExitRequested;

        _view.Hide();
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    private void StartGameRequested()
    {
        _fsm.Signal(new ToGameplaySignal(GameplayStateData.GetNewGameState(_gameSettings.Difficult.Value, 
            _inputDispatcher.PlayerInputSources.Count)));
    }

    private void GameExitRequested()
    {
        Application.Quit();
    }

    private void SettingsRequested()
    {
        _fsm.Signal(ToSettingsSignal.Empty);
    }

    private void LoadMenuRequested()
    {
        _fsm.Signal(ToLoadMenuSignal.Empty);
    }

    void ISignalHandler<ToGameplaySignal>.Handle(ToGameplaySignal signal)
    {
        _fsm.CallTransition<MainMenuState, LoadGameplayState>(new MainMenuToGameplayExitData(signal.GameplayState));
    }

    void ISignalHandler<ToLoadMenuSignal>.Handle(ToLoadMenuSignal signal)
    {
        _fsm.CallTransition<MainMenuState, LoadMenuState>(StateExitData.Empty);
    }

    void ISignalHandler<ToSettingsSignal>.Handle(ToSettingsSignal signal)
    {
        _fsm.CallTransition<MainMenuState, SettingsState>(StateExitData.Empty);
    }
}
