using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuToSettingsTransition : IStateTransition<MainMenuState, SettingsState>
{
    public MainMenuState FromState { get; }

    public SettingsState ToState { get; }

    public MainMenuToSettingsTransition(MainMenuState from, SettingsState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        return StateEnterData.Empty;
    }
}
