using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditsView : HFSMStateView
{
    public event Action BackToMenuRequested;

    [SerializeField, Min(0.1f)] private float _duration;

    [SerializeField] private ControlGuide _controlGuide;

    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private Scrollbar _bar;

    private InputDispatcher _dispatcher;

    private float _factor;

    public void Initialize(InputDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        _bar.interactable = false;

        _factor = 1.0f;

        _controlGuide.Show();

        TryLaunchCoroutineWithStateLifeSpan(_ShowTitles());
    }

    public void Hide()
    {
        _controlGuide.Hide();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        _factor = 1.0f;

        if (_dispatcher.UIInput.UIInputProvider.SpeedUpCreditsButton)
        {
            _factor = 5.5f;
        }
        else if (_dispatcher.UIInput.UIInputProvider.IsBackButtonDown)
        {
            AudioPlayer.Instance?.PlayNavigationBack();

            BackToMenuRequested?.Invoke();
        }
    }

    private IEnumerator<float> _ShowTitles()
    {
        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / _duration * _factor)
        {
            _scrollRect.verticalNormalizedPosition = 1.0f - i;

            yield return Timing.WaitForOneFrame;
        }

        _scrollRect.verticalNormalizedPosition = 0.0f;

        _bar.interactable = true;

        EventSystem.current?.SetSelectedGameObject(_bar.gameObject);
    }
}
