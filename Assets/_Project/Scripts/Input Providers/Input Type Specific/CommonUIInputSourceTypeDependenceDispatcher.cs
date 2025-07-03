using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUIInputSourceTypeDependenceDispatcher : IDisposable
{
    private CommonUIInputSourceTypeDependence[] _dependences;

    private ReadOnlyReactiveProperty<InputSource.SourceType> _inputType;

    public CommonUIInputSourceTypeDependenceDispatcher(CommonUIInputSourceTypeDependence[] allDependences,
        InputDispatcher inputDispatcher)
    {
        _dependences = allDependences;

        _inputType = inputDispatcher.UIInputType;

        inputDispatcher.UIInputType.Subscribe(OnTypeChanged);
    }

    private void OnTypeChanged(InputSource.SourceType type)
    {
        for (int i = 0; i < _dependences.Length; i++)
        {
            _dependences[i].InputTypeChanged(type);
        }
    }

    public void Dispose()
    {
        if (_inputType != null)
        {
            _inputType.Unsubscribe(OnTypeChanged);

            _inputType = null;
        }

        _dependences = null;
    }
}
