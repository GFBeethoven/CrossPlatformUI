using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameplayToLostControllerTransition : IStateTransition<GameplayState, LostControllerState>
{
    public GameplayState FromState { get; }

    public LostControllerState ToState { get; }

    public GameplayToLostControllerTransition(GameplayState from, LostControllerState to)
    {
        FromState = from;
        ToState = to;
    }

    public StateEnterData Handle(StateExitData data)
    {
        var conreteData = data.As<GameplayExitData>();

        return new LostControllerEnterData(conreteData.StateOnExit, conreteData.TempThumbnail);
    }
}
