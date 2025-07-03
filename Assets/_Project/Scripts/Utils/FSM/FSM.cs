using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    public event Action OnStateChanged;

    private IState _currentState = null;

    private Dictionary<Type, IStateTransition[]> _transitions;

    public FSM(IState[] allStates, Dictionary<Type, IStateTransition[]> transitions,
        IState initState, StateEnterData initEnterData)
    {
        _transitions = transitions;

        for (int i = 0; i < allStates.Length; i++)
        {
            allStates[i].Initialize(this);
        }

        ChangeState(initState, initEnterData);
    }

    public bool IsCurrentState(IState state)
    {
        return _currentState == state;
    }

    public void Signal<T>(T signal) where T : Signal
    {
        if (_currentState == null) return;

        if (_currentState is ISignalHandler<T> handler)
        {
            handler.Handle(signal);           
        }
    }

    public void CallTransition<TFromState, TToState>(StateExitData exitData) where TFromState : IState 
        where TToState : IState
    {
        IStateTransition[] transitions = _transitions[typeof(TFromState)];

        for (int i = 0; i < transitions.Length; i++)
        {
            if (transitions[i] is IStateTransition<TFromState, TToState> transition)
            {
                ChangeState(transition.ToState, transition.Handle(exitData));

                return;
            }
        }
    }

    public void Update()
    {
        _currentState?.Update();
    }

    public void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }

    private void ChangeState(IState state, StateEnterData enterData)
    {
        _currentState?.Exit();
        _currentState = state;
        _currentState.Enter(enterData);

        OnStateChanged?.Invoke();
    }
}
