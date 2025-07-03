using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSoundEmitter : MonoBehaviour
{
    private Slider _slider;
    public Slider Slider
    {
        get
        {
            if (_slider == null)
            {
                _slider = GetComponent<Slider>();
            }

            return _slider;
        }
    }

    private void OnEnable()
    {
        Slider.onValueChanged.AddListener(Emit);
    }

    private void OnDisable()
    {
        Slider.onValueChanged.RemoveListener(Emit);
    }

    private void Emit(float value)
    {
        AudioPlayer.Instance?.PlaySlider();
    }
}
