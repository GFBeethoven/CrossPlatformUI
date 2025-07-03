using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSoundEmitter : MonoBehaviour
{
    private Toggle _toggle;
    public Toggle Toggle
    {
        get
        {
            if (_toggle == null)
            {
                _toggle = GetComponent<Toggle>();
            }

            return _toggle;
        }
    }

    private void OnEnable()
    {
        Toggle.onValueChanged.AddListener(Emit);
    }

    private void OnDisable()
    {
        Toggle.onValueChanged.RemoveListener(Emit);
    }

    private void Emit(bool value)
    {
        AudioPlayer.Instance?.PlayToggle();
    }
}
