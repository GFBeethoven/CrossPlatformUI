using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PulseImage : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float _period;

    [SerializeField] private Color _colorA = Color.white;
    [SerializeField] private Color _colorB = Color.white;

    private Image Image
    {
        get
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }

            return _image;
        }
    }

    private Image _image;

    private CoroutineHandle _coroutineHandle;

    private void OnEnable()
    {
        if (_coroutineHandle.IsValid)
        {
            Timing.KillCoroutines(_coroutineHandle);
        }

        _coroutineHandle = Timing.RunCoroutine(_Pulse().CancelWith(gameObject));
    }

    private void OnDisable()
    {
        if (_coroutineHandle.IsValid)
        {
            Timing.KillCoroutines(_coroutineHandle);
        }
    }

    private IEnumerator<float> _Pulse()
    {
        while (true)
        {
            for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / _period)
            {
                float t = (Mathf.Sin(i * 2.0f * Mathf.PI) + 1.0f) / 2.0f;

                Image.color = Color.Lerp(_colorA, _colorB, t);

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
