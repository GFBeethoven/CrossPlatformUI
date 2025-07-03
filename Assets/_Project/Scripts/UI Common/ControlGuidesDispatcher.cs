using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlGuidesDispatcher : MonoBehaviour
{
    [SerializeField] private Image _bottomStrip;

    private ControlGuide[] _allControlGuides;

    private bool _isInitialized = false;

    private void Initialize()
    {
        if (_isInitialized) return;

        _allControlGuides = GetComponentsInChildren<ControlGuide>();

        _isInitialized = true;
    }

    public void ShowGuide(ControlGuide control)
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        for (int i = 0; i < _allControlGuides.Length; i++)
        {
            _allControlGuides[i].gameObject.SetActive(false);
        }

        if (control != null)
        {
            control.gameObject.SetActive(true);

            _bottomStrip.gameObject.SetActive(true);
        }
        else
        {
            _bottomStrip.gameObject.SetActive(false);
        }
    }
}
