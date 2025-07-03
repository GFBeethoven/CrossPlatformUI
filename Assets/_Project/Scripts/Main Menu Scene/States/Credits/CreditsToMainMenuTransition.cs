using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsToMainMenuTransition : IStateTransition<CreditsState, MainMenuState>
{
    public CreditsState FromState { get; }

    public MainMenuState ToState { get; }

    public CreditsToMainMenuTransition(CreditsState from, MainMenuState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        return StateEnterData.Empty;
    }
}
