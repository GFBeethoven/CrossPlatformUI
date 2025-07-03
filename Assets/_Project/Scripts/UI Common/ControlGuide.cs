using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGuide : MonoBehaviour
{
    private ControlGuidesDispatcher Dispatcher
    {
        get
        {
            if (_cached == null)
            {
                _cached = GetComponentInParent<ControlGuidesDispatcher>();  

                if (_cached == null)
                {
                    throw new ApplicationException("Control Guide should be child of Control Guide Dispatcher");
                }
            }

            return _cached;
        }
    }

    private ControlGuidesDispatcher _cached;

    public void Show()
    {
        Dispatcher.ShowGuide(this);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
