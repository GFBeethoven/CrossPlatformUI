using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuToGameplayTransition : IStateTransition<MainMenuState, LoadGameplayState>
{
    public MainMenuState FromState { get; }

    public LoadGameplayState ToState { get; }

    public MainMenuToGameplayTransition(MainMenuState from, LoadGameplayState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        var concreteData = data.As<MainMenuToGameplayExitData>();

        return new GameplayEnterData(concreteData.GameplayStateData);
    }
}
