using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PauseEnterData : StateEnterData
{
    public string Thumbnail { get; }

    public GameplayStateData PausedState { get; }

    public InputSource CalledInputSource { get; }

    public PauseEnterData(GameplayStateData pausedState, string thumbnail, InputSource calledInputSource)
    {
        PausedState = pausedState;
        Thumbnail = thumbnail;
        CalledInputSource = calledInputSource;
    }
}
