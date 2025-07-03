using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuToLoadMenuTransition : IStateTransition<MainMenuState, LoadMenuState>
{
    public MainMenuState FromState { get; }

    public LoadMenuState ToState { get; }

    public MainMenuToLoadMenuTransition(MainMenuState from, LoadMenuState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        return StateEnterData.Empty;
    }
}
