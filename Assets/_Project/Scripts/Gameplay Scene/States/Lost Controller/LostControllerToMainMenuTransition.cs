using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LostControllerToMainMenuTransition : IStateTransition<LostControllerState, LoadMainMenuState>
{
    public LostControllerState FromState { get; }

    public LoadMainMenuState ToState { get; }

    public LostControllerToMainMenuTransition(LostControllerState from, LoadMainMenuState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        var conreteData = data.As<LostControllerExitData>();

        return new LoadMainMenuEnterData(conreteData.PausedState, conreteData.Thumbnail);
    }
}
