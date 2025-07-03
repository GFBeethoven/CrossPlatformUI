using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameplayToPauseTransition : IStateTransition<GameplayState, PauseState>
{
    public GameplayState FromState { get; }

    public PauseState ToState { get; }

    public GameplayToPauseTransition(GameplayState from, PauseState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        var conreteData = data.As<GameplayExitData>();

        return new PauseEnterData(conreteData.StateOnExit, conreteData.TempThumbnail, conreteData.Invoker);
    }
}
