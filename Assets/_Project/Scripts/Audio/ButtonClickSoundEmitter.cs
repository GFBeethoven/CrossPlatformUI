using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSoundEmitter : MonoBehaviour
{
    private Button _button;
    public Button Button
    {
        get
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }

            return _button;
        }
    }

    private void OnEnable()
    {
        Button.onClick.AddListener(Emit);
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(Emit);
    }

    private void Emit()
    {
        AudioPlayer.Instance?.PlayButtonClick();
    }
}
