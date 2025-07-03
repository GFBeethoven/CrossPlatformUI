using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TitleState : HFSMState, ISignalHandler<ToMainMenuSignal>
{
    private TitleScreenView _view;

    public TitleState(TitleScreenView rootView, InputDispatcher inputDispatcher) : base(null, rootView)
    {
        _view = rootView;
        _view.Setup(inputDispatcher);
    }

    public override void Enter(StateEnterData enterData)
    {
        _view.NextScreenRequested += NextScreenRequested;

        _view.Show();
    }

    public override void Exit()
    {
        _view.NextScreenRequested -= NextScreenRequested;

        _view.Hide();
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    private void NextScreenRequested()
    {
        _fsm.Signal(ToMainMenuSignal.Empty);
    }

    void ISignalHandler<ToMainMenuSignal>.Handle(ToMainMenuSignal signal)
    {
        _fsm.CallTransition<TitleState, MainMenuState>(StateExitData.Empty);
    }
}
