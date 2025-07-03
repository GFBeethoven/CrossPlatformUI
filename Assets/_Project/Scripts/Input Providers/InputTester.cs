using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTester : MonoBehaviour
{
    private InputSource _uiInput;

    public void Setup(InputSource source)
    {
        _uiInput = source;
    }

    private void Update()
    {
        if (_uiInput == null) return;


        if (_uiInput.UIInputProvider.IsToLeftTabButtonDown) Debug.Log("Left Tab");
        if (_uiInput.UIInputProvider.IsToRightTabButtonDown) Debug.Log("Right Tab");
    }
}
