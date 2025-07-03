using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class LoadSlotView : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private LeanLocalToken _scoreToken;
    [SerializeField] private LeanLocalToken _creationTime;

    public Texture2D Thumbnail { get; set; }

    private RectTransform _rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>(); 
            }

            return _rectTransform;
        }
    }

    public LoadMenuView EventsHandler { get; set; }

    public void Initialize()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            EventsHandler?.HandleSlotClick(this);
        });
    }

    public void SetBestScore(int score)
    {
        _scoreToken.SetValue(score);
    }

    public void SetCreationTime(DateTime dateTime)
    {
        _creationTime.Value = dateTime.ToShortDateString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventsHandler?.HandleSlotPointerEnter(this);
    }
}
