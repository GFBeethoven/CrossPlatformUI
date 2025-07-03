using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToPauseSignal : Signal
{
    public InputSource Invoker { get; }

    public ToPauseSignal(InputSource invoker)
    {
        Invoker = invoker;
    }
}

public class RequiredNewInputSourceSignal : Signal
{
    public InputSource Invoker { get; }

    public RequiredNewInputSourceSignal(InputSource invoker)
    {
        Invoker = invoker;
    }
}