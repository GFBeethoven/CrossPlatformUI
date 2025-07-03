using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsState : HFSMState, ISignalHandler<ToMainMenuSignal>, ISignalHandler<ToCreditsSignal>
{
    private GameSettings _settings;
    private GameSettingsView _gameSettingsView;

    private ISaveLoadSystem _saveLoadSystem;

    public SettingsState(GameSettings settings, GameSettingsView view, InputDispatcher inputDispatcher,
        ISaveLoadSystem saveLoadSystem) : base(null, view)
    {
        _settings = settings;

        _saveLoadSystem = saveLoadSystem;

        _gameSettingsView = view;
        _gameSettingsView.Initialize(settings, inputDispatcher, saveLoadSystem.SavedStateCount);
    }

    public override void Enter(StateEnterData enterData)
    {
        _gameSettingsView.BackToMainMenuRequested += BackToMainMenu;
        _gameSettingsView.CreditsRequested += CreditsRequested;
        _gameSettingsView.DeleteAllSavedRequested += DeleteAllSavedRequested;

        _gameSettingsView.Show();
    }

    private void DeleteAllSavedRequested()
    {
        _saveLoadSystem.DeleteAllGameplayStates();
    }

    private void CreditsRequested()
    {
        _fsm.Signal(ToCreditsSignal.Empty);
    }

    public override void Exit()
    {
        _settings.Save();

        _gameSettingsView.BackToMainMenuRequested -= BackToMainMenu;
        _gameSettingsView.CreditsRequested -= CreditsRequested;
        _gameSettingsView.DeleteAllSavedRequested -= DeleteAllSavedRequested;

        _gameSettingsView.Hide();
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    private void BackToMainMenu()
    {
        _fsm.Signal(ToMainMenuSignal.Empty);
    }

    void ISignalHandler<ToMainMenuSignal>.Handle(ToMainMenuSignal signal)
    {
        _fsm.CallTransition<SettingsState, MainMenuState>(StateExitData.Empty);
    }

    void ISignalHandler<ToCreditsSignal>.Handle(ToCreditsSignal signal)
    {
        _fsm.CallTransition<SettingsState, CreditsState>(StateExitData.Empty);
    }
}
