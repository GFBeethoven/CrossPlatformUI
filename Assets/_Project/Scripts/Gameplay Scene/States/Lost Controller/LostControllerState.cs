using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LostControllerState : HFSMState, ISignalHandler<ToGameplaySignal>, ISignalHandler<ToMainMenuSignal>
{
    private LostControllerView _view;
    
    private LostControllerEnterData _enterData;

    public LostControllerState(LostControllerView view) : base(null, view)
    {
        _view = view;
    }

    public override void Enter(StateEnterData enterData)
    {
        _enterData = enterData.As<LostControllerEnterData>();   

        _view.Show();
    }

    public override void Exit()
    {
        _view.Hide();

        _enterData = null;
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    void ISignalHandler<ToMainMenuSignal>.Handle(ToMainMenuSignal signal)
    {
        _fsm.CallTransition<LostControllerState, LoadMainMenuState>(new LostControllerExitData(_enterData?.PausedState, _enterData?.Thumbnail));
    }

    void ISignalHandler<ToGameplaySignal>.Handle(ToGameplaySignal signal)
    {
        _fsm.CallTransition<LostControllerState, GameplayState>(new LostControllerExitData(_enterData?.PausedState, _enterData?.Thumbnail));
    }
}
