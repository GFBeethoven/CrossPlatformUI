using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class TitleScreenView : HFSMStateView
{
    public event Action NextScreenRequested;

    private InputDispatcher _inputDispatcher;

    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private RectTransform _leftCurtain;
    [SerializeField] private RectTransform _rightCurtain;
    [SerializeField] private RectTransform _title;

    private bool _keyHandled;

    public void Setup(InputDispatcher dispatcher)
    {
        _inputDispatcher = dispatcher;
    }

    public void Show()
    {
        _keyHandled = false;

        gameObject.SetActive(true);

        TryLaunchCoroutineWithStateLifeSpan(_PulseLabel());
        TryLaunchCoroutineWithStateLifeSpan(_ShowCurtains());
        TryLaunchCoroutineWithStateLifeSpan(_ShowTitle());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_inputDispatcher == null || _keyHandled) return;
        
        if (_inputDispatcher.UIInput.UIInputProvider.IsAnyButtonDown)
        {
            AudioPlayer.Instance?.PlayTitleScreenPass();

            Timing.RunCoroutine(_BounceLabel(() => NextScreenRequested?.Invoke()));

            _keyHandled = true;
        }
    }

    private IEnumerator<float> _BounceLabel(Action endCallback)
    {
        const float Duration = 0.75f;
        const float UpScale = 3.0f;

        Color color = Color.white;
        Color endColor = Color.white;
        endColor.a = 0.0f;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            _label.rectTransform.localScale = Vector3.one * Mathf.Lerp(1.0f, UpScale, Easing.InOutQuad(i));
            _label.color = Color.Lerp(color, endColor, Easing.OutQuad(i));

            yield return Timing.WaitForOneFrame;
        }

        _label.gameObject.SetActive(false);

        endCallback?.Invoke();
    }

    private IEnumerator<float> _PulseLabel()
    {
        Color a = Color.white;
        Color b = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        float t = 0.0f;

        while (true)
        {
            if (_keyHandled) yield break;

            float T = (Mathf.Sin(t * 2.0f * Mathf.PI) + 1.0f) / 2.0f;

            _label.color = Color.Lerp(a, b, T);

            t += Timing.DeltaTime;

            yield return Timing.WaitForOneFrame;
        }
    }

    private IEnumerator<float> _ShowCurtains()
    {
        const float Duration = 2.0f;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            float t = Easing.OutQuad(i);

            _leftCurtain.anchoredPosition = new Vector2(Mathf.Lerp(-200.0f, 124.0f, t), 15.0f);
            _rightCurtain.anchoredPosition = new Vector2(Mathf.Lerp(200.0f, -124.0f, t), 15.0f);

            yield return Timing.WaitForOneFrame;
        }

        _leftCurtain.anchoredPosition = new Vector2(124.0f, 15.0f);
        _rightCurtain.anchoredPosition = new Vector2(-124.0f, 15.0f);
    }

    private IEnumerator<float> _ShowTitle()
    {
        const float UpY = 700.0f;
        const float Y = 191.0f;
        const float Duration = 1.0f;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            _title.anchoredPosition = new Vector2(0.0f, Mathf.LerpUnclamped(UpY, Y, Easing.OutBack(i)));

            yield return Timing.WaitForOneFrame;
        }

        _title.anchoredPosition = new Vector2(0.0f, Y);
    }
}
