using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleToMainMenuTransition : IStateTransition<TitleState, MainMenuState>
{
    public TitleState FromState { get; }

    public MainMenuState ToState { get; }

    public TitleToMainMenuTransition(TitleState from, MainMenuState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        return StateEnterData.Empty;
    }
}
