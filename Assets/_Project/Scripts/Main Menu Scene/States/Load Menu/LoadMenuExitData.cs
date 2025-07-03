using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LoadMenuExitData : StateExitData
{
    public GameplayStateData StateToLoad { get; }

    public LoadMenuExitData(GameplayStateData stateToLoad)
    {
        StateToLoad = stateToLoad;
    }
}
