using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CreditsState : HFSMState, ISignalHandler<ToMainMenuSignal>
{
    private CreditsView _rootGameObject;

    public CreditsState(CreditsView view, InputDispatcher inputDispatcher) : base(null, view)
    {
        _rootGameObject = view;

        _rootGameObject.Initialize(inputDispatcher);
    }

    public override void Enter(StateEnterData enterData)
    {
        _rootGameObject.BackToMenuRequested += BackToMenuRequested;
        _rootGameObject.Show();
    }

    public override void Exit()
    {
        _rootGameObject.BackToMenuRequested -= BackToMenuRequested;
        _rootGameObject.Hide();
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    private void BackToMenuRequested()
    {
        _fsm.Signal(ToMainMenuSignal.Empty);
    }

    void ISignalHandler<ToMainMenuSignal>.Handle(ToMainMenuSignal signal)
    {
        _fsm.CallTransition<CreditsState, MainMenuState>(StateExitData.Empty);
    }
}
