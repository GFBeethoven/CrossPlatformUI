using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameplayState : HFSMState, ISignalHandler<ToPauseSignal>, ISignalHandler<RequiredNewInputSourceSignal>
{
    private GameplayView _view;

    private GameplayModel _model;
    private GameplaySplitScreen _splitScreen;
    private GameplayViewModel _viewModel;

    public GameplayState(GameplayView view, GameplayModel model, GameplayViewModel viewModel,
        GameplaySplitScreen splitScreen, InputDispatcher inputDispatcher, GameSettings gameSettings) : base(null, view)
    {
        _view = view;
        _model = model;
        _splitScreen = splitScreen;
        _viewModel = viewModel;

        _splitScreen.Setup(viewModel, inputDispatcher, gameSettings);
    }

    public override void Enter(StateEnterData enterData)
    {
        var concreteData = enterData.As<GameplayEnterData>();

        _viewModel.PauseRequested += PauseRequested;

        _view.Show();

        _model.Setup(concreteData.State);
    }

    public override void Exit()
    {
        _viewModel.PauseRequested -= PauseRequested;

        _view.Hide();
    }

    public override void Update()
    {
        _model.Tick();
        _splitScreen.Tick();
    }

    public override void FixedUpdate() { }

    private void PauseRequested(InputSource invoker)
    {
        _fsm.Signal(new ToPauseSignal(invoker));
    }

    void ISignalHandler<ToPauseSignal>.Handle(ToPauseSignal signal)
    {
        Timing.RunCoroutine(_DelayedTransition(signal));
    }

    void ISignalHandler<RequiredNewInputSourceSignal>.Handle(RequiredNewInputSourceSignal signal)
    {
        Timing.RunCoroutine(_DelayedTransition(signal));
    }

    private IEnumerator<float> _DelayedTransition(ToPauseSignal signal)
    {
        string capture = TempThumbnailCapture.Take();

        yield return Timing.WaitForOneFrame;
        yield return Timing.WaitForOneFrame;

        _fsm.CallTransition<GameplayState, PauseState>(new GameplayExitData(_model.GetGameplayStateCopy(),
            capture, signal.Invoker));
    }

    private IEnumerator<float> _DelayedTransition(RequiredNewInputSourceSignal signal)
    {
        string capture = TempThumbnailCapture.Take();

        yield return Timing.WaitForOneFrame;

        _fsm.CallTransition<GameplayState, LostControllerState>(new GameplayExitData(_model.GetGameplayStateCopy(),
            capture, signal.Invoker));
    }
}
