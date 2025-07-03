using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class PauseState : HFSMState, ISignalHandler<ToGameplaySignal>, ISignalHandler<ToMainMenuSignal>
{
    private PauseView _pauseView;

    private PauseEnterData _currentEnterData;

    private ISaveLoadSystem _saveLoadSystem;

    public PauseState(PauseView view, ISaveLoadSystem saveLoadSystem) : base(null, view)
    {
        _pauseView = view;
        _pauseView.Initialize();

        _saveLoadSystem = saveLoadSystem;
    }

    public override void Enter(StateEnterData enterData)
    {
        _currentEnterData = enterData.As<PauseEnterData>();

        _pauseView.UnpauseRequested += UnpauseRequested;
        _pauseView.MainMenuRequested += MainMenuRequested;
        _pauseView.SaveStateRequested += SaveStateRequested;

        _pauseView.Show(_currentEnterData.CalledInputSource);
    }

    public override void Exit()
    {
        _pauseView.Hide();

        _pauseView.UnpauseRequested -= UnpauseRequested;
        _pauseView.MainMenuRequested -= MainMenuRequested;
        _pauseView.SaveStateRequested -= SaveStateRequested;

        _currentEnterData = null;
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    private void SaveStateRequested()
    {
        if (_currentEnterData?.PausedState != null) ;

        _saveLoadSystem.SaveGameplayState(_currentEnterData.PausedState, _currentEnterData.Thumbnail);
    }

    private void MainMenuRequested()
    {
        _fsm.Signal(ToMainMenuSignal.Empty);
    }

    private void UnpauseRequested()
    {
        _fsm.Signal(new ToGameplaySignal(_currentEnterData.PausedState));
    }

    void ISignalHandler<ToMainMenuSignal>.Handle(ToMainMenuSignal signal)
    {
        _fsm.CallTransition<PauseState, LoadMainMenuState>(new PauseExitData(_currentEnterData?.PausedState, _currentEnterData?.Thumbnail));
    }

    void ISignalHandler<ToGameplaySignal>.Handle(ToGameplaySignal signal)
    {
        _fsm.CallTransition<PauseState, GameplayState>(new PauseExitData(_currentEnterData?.PausedState, _currentEnterData?.Thumbnail));
    }
}
