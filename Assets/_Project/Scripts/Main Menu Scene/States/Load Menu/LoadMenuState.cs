using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LoadMenuState : HFSMState, ISignalHandler<ToMainMenuSignal>, ISignalHandler<ToGameplaySignal>
{
    private LoadMenuModel _model;
    private LoadMenuViewModel _viewModel;
    private LoadMenuView _view;

    public LoadMenuState(LoadMenuModel model, LoadMenuViewModel viewModel,
        LoadMenuView view, InputDispatcher inputDispatcher) : base(null, view)
    {
        _model = model;
        _viewModel = viewModel;
        _view = view;
        _view.Setup(_viewModel, inputDispatcher);
    }

    public override void Enter(StateEnterData enterData)
    {
        _viewModel.OnLoadStateRequsted += OnLoadRequested;
        _viewModel.OnMainMenuButtonClicked += ToMainMenu;

        _model.Refresh();

        _view.Show();
    }

    public override void Exit()
    {
        _viewModel.OnLoadStateRequsted -= OnLoadRequested;
        _viewModel.OnMainMenuButtonClicked -= ToMainMenu;

        _view.Hide();
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    private void OnLoadRequested(GameplayStateData state)
    {
        _fsm.Signal(new ToGameplaySignal(state));
    }

    private void ToMainMenu()
    {
        _fsm.Signal(ToMainMenuSignal.Empty);
    }

    void ISignalHandler<ToMainMenuSignal>.Handle(ToMainMenuSignal signal)
    {
        _fsm.CallTransition<LoadMenuState, MainMenuState>(StateExitData.Empty);
    }

    void ISignalHandler<ToGameplaySignal>.Handle(ToGameplaySignal signal)
    {
        _fsm.CallTransition<LoadMenuState, LoadGameplayState>(new LoadMenuExitData(signal.GameplayState));
    }
}
