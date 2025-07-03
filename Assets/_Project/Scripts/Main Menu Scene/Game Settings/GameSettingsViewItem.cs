using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class GameSettingsViewItem : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [field: SerializeField] public string DescriptionTitleKey { get; private set; }
    [field: SerializeField] public string DescriptionKey { get; private set; }
    [field: SerializeField] public ControlGuide ControlGuide { get; private set; }

    [SerializeField] private Image _background;
    [SerializeField] private Color _selectedColor = Color.white;
    [SerializeField] private Color _deselectedColor = Color.white;

    private RectTransform _rectTransform;
    public RectTransform ParentRectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>()?.parent?.GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }

    private void Awake()
    {
        _background.color = _deselectedColor;
    }

    private GameSettingsViewTab _view;

    public void Setup(GameSettingsViewTab view)
    {
        _view = view;
    }

    public void ShowDeselected()
    {
        OnDeselect();
    }

    protected virtual void OnSelect()
    {
        _background.color = _selectedColor;
    }

    protected virtual void OnDeselect()
    {
        _background.color = _deselectedColor;
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        _view?.HandleItemSelect(this);

        OnSelect();
    }

    void IDeselectHandler.OnDeselect(BaseEventData eventData)
    {
        _view?.HandleItemDeselect(this);

        OnDeselect();
    }
}
