using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HFSMStateView : MonoBehaviour
{
    private HFSMState _associatedState;

    public void Setup(HFSMState hfsmState)
    {
        _associatedState = hfsmState;
    }

    protected void TryLaunchCoroutineWithStateLifeSpan(IEnumerator<float> coroutine)
    {
        if (_associatedState == null)
        {
            Debug.LogWarning("Cannot launch coroutine with state life span - not assigned state (null)");
        }

        _associatedState?.LaunchCoroutineWithStateLifeSpan(coroutine.CancelWith(gameObject));
    }
}
