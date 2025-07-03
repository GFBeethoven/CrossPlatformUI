using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateTransition
{
    public StateEnterData Handle(StateExitData data);
}

public interface IStateTransition<TFromState, TToState> : IStateTransition
    where TFromState : IState where TToState : IState
{
    public TFromState FromState { get; }

    public TToState ToState { get; }
}
