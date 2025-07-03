using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMenuToGameplayTransition : IStateTransition<LoadMenuState, LoadGameplayState>
{
    public LoadMenuState FromState { get; }

    public LoadGameplayState ToState { get; }

    public LoadMenuToGameplayTransition(LoadMenuState from, LoadGameplayState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        var concreteData = data.As<LoadMenuExitData>();

        return new GameplayEnterData(concreteData.StateToLoad);
    }
}
