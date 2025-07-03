using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsToMainMenuTransition : IStateTransition<SettingsState, MainMenuState>
{
    public SettingsState FromState { get; }

    public MainMenuState ToState { get; }

    public SettingsToMainMenuTransition(SettingsState from, MainMenuState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        return StateEnterData.Empty;
    }
}
