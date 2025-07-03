using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class DownUpTransitionCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform _overlap;

    private CoroutineHandle _coroutine;

    public void Show()
    {
        gameObject.SetActive(true);

        if (_coroutine.IsValid)
        {
            Timing.KillCoroutines(_coroutine);
        }

        _overlap.anchoredPosition = Vector2.zero;

        _coroutine = Timing.RunCoroutine(_Overlap().CancelWith(gameObject));
    }

    private IEnumerator<float> _Overlap()
    {
        const float Duration = 0.5f;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            _overlap.anchoredPosition = new Vector2(0.0f, Mathf.Lerp(0.0f, 1200.0f, Easing.OutQuad(i)));

            yield return Timing.WaitForOneFrame;
        }

        gameObject.SetActive(false);

        _coroutine = new CoroutineHandle();
    }
}
