using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HFSMState : IState
{
    protected FSM _fsm;

    private HFSMState _parent;
    private HFSMState _subState;

    private HFSMState _stateToUpdate;

    private List<CoroutineHandle> _disposableOnExitCoroutines;

    public HFSMState(HFSMState subState, HFSMStateView view)
    {
        if (view != null)
        {
            view.Setup(this);
        }

        _disposableOnExitCoroutines = new();

        _subState = subState;

        if (_subState != null)
        {
            _subState._parent = this;
        }

        _stateToUpdate = this;
    }

    public virtual void Initialize(FSM fsm)
    {
        _fsm = fsm;
    }

    public void LaunchCoroutineWithStateLifeSpan(IEnumerator<float> coroutine)
    {
        if (coroutine == null) return;

        if (_fsm.IsCurrentState(this) == false) return;

        _disposableOnExitCoroutines.Add(Timing.RunCoroutine(coroutine));
    }

    protected void GoToSubState(StateEnterData enterData)
    {
        if (_subState == null) return;

        if (_subState == _stateToUpdate) return;

        _stateToUpdate = _subState;

        _stateToUpdate.Enter(enterData);
    }

    protected void ReturnToParentState()
    {
        if (_parent == null) return;

        Exit();

        _parent._stateToUpdate = _parent;
    }

    public abstract void Enter(StateEnterData enterData);
    public abstract void Exit();
    public abstract void Update();
    public abstract void FixedUpdate();

    void IState.Initialize(FSM fsm)
    {
        Initialize(fsm);
    }

    void IState.Enter(StateEnterData enterData)
    {
        _stateToUpdate = this;

        Enter(enterData);
    }

    void IState.Exit()
    {
        if (_subState == _stateToUpdate)
        {
            _subState.Exit();
        }

        _stateToUpdate = this;

        for (int i = 0; i < _disposableOnExitCoroutines.Count; i++)
        {
            if (_disposableOnExitCoroutines[i].IsValid)
            {
                Timing.KillCoroutines(_disposableOnExitCoroutines[i]);
            }
        }

        _disposableOnExitCoroutines.Clear();

        Exit();
    }

    void IState.Update()
    {
        _stateToUpdate.Update();
    }

    void IState.FixedUpdate()
    {
        _stateToUpdate.FixedUpdate();
    }
}
