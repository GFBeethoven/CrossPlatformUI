using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameplayExitData : StateExitData
{
    public string TempThumbnail { get; }

    public GameplayStateData StateOnExit { get; }

    public InputSource Invoker { get; }

    public GameplayExitData(GameplayStateData stateOnExit, string tempThumbnail, InputSource invoker)
    {
        StateOnExit = stateOnExit;
        TempThumbnail = tempThumbnail;
        Invoker = invoker;
    }
}
