using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PauseToMainManuTransition : IStateTransition<PauseState, LoadMainMenuState>
{
    public PauseState FromState { get; }

    public LoadMainMenuState ToState { get; }

    public PauseToMainManuTransition(PauseState from, LoadMainMenuState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        var conreteData = data.As<PauseExitData>();

        return new LoadMainMenuEnterData(conreteData.PausedState, conreteData.Thumbnail);
    }
}
