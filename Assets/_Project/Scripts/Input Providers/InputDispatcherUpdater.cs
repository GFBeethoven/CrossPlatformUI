using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDispatcherUpdater : MonoBehaviour
{
    private InputDispatcher _inputDispatcher;

    public void Setup(InputDispatcher inputDispatcher)
    {
        _inputDispatcher = inputDispatcher;
    }

    private void Update()
    {
        _inputDispatcher?.Tick();
    }
}
