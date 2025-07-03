using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PauseExitData : StateExitData
{
    public string Thumbnail { get; }

    public GameplayStateData PausedState { get; }

    public PauseExitData(GameplayStateData pausedState, string thumbnail)
    {
        PausedState = pausedState;
        Thumbnail = thumbnail;
    }
}
