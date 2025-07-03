using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsToCreditsTransition : IStateTransition<SettingsState, CreditsState>
{
    public SettingsState FromState { get; }

    public CreditsState ToState { get; }

    public SettingsToCreditsTransition(SettingsState from, CreditsState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        return StateEnterData.Empty;
    }
}
