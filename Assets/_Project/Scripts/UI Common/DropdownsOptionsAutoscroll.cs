using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class DropdownsOptionsAutoscroll : MonoBehaviour
{
    private GameObject _prevSelected;

    private ScrollRect _scrollRect;
    private ScrollRect ScrollRect
    {
        get
        {
            if (_scrollRect == null)
            {
                _scrollRect = GetComponent<ScrollRect>();
            }

            return _scrollRect;
        }
    }

    private void OnEnable()
    {
        _prevSelected = null;
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != _prevSelected)
        {
            _prevSelected = EventSystem.current.currentSelectedGameObject;

            var rect = _prevSelected.GetComponent<RectTransform>();

            if (rect != null)
            {
                ScrollRect.ScrollToShow(rect);
            }
        }
    }
}
