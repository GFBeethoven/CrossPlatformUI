using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LostControllerExitData : StateExitData
{
    public string Thumbnail { get; }

    public GameplayStateData PausedState { get; }

    public LostControllerExitData(GameplayStateData pausedState, string thumbnail)
    {
        PausedState = pausedState;
        Thumbnail = thumbnail;
    }
}
