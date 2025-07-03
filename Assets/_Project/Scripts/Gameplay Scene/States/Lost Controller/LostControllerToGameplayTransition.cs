using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LostControllerToGameplayTransition : IStateTransition<LostControllerState, GameplayState>
{
    public LostControllerState FromState { get; }

    public GameplayState ToState { get; }

    public LostControllerToGameplayTransition(LostControllerState from, GameplayState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        var conreteData = data.As<LostControllerExitData>();

        return new GameplayEnterData(conreteData.PausedState);
    }
}
