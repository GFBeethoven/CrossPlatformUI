using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEnterData
{
    public readonly static StateEnterData Empty = new StateEnterData();

    public T As<T>() where T : StateEnterData
    {
        return this as T;
    }
}
