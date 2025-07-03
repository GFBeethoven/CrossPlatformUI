using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class StateExitData
{
    public readonly static StateExitData Empty = new StateExitData();

    public T As<T>() where T : StateExitData
    {
        return this as T;
    }
}
