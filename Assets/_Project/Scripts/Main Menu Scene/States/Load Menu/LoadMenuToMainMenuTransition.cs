using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMenuToMainMenuTransition : IStateTransition<LoadMenuState, MainMenuState>
{
    public LoadMenuState FromState { get; }

    public MainMenuState ToState { get; }

    public LoadMenuToMainMenuTransition(LoadMenuState from, MainMenuState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        return StateEnterData.Empty;
    }
}
