using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PauseToGameplayTransition : IStateTransition<PauseState, GameplayState>
{
    public PauseState FromState { get; }

    public GameplayState ToState { get; }

    public PauseToGameplayTransition(PauseState from, GameplayState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        var conreteData = data.As<PauseExitData>();

        return new GameplayEnterData(conreteData.PausedState);
    }
}
