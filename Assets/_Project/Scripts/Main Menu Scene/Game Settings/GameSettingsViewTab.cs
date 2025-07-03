using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class GameSettingsViewTab : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;

    [field: SerializeField] public GameSettingsViewTabButton AssociatedButton { get; private set; }

    public Selectable FirstObjectToSelect => _firstToSelect;
    [SerializeField] private Selectable _firstToSelect;

    private GameSettingsView _view;

    private EventSystem _eventSystem;

    private ReadOnlyReactiveProperty<InputSource.SourceType> _inputType;

    private GameSettingsViewItem[] _items;

    public virtual void Initialize(GameSettings model, GameSettingsView view, EventSystem eventSystem,
        ReadOnlyReactiveProperty<InputSource.SourceType> inputType)
    {
        _inputType = inputType;

        _eventSystem = eventSystem;

        _view = view;

        _items = GetComponentsInChildren<GameSettingsViewItem>();

        if (_items != null)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                _items[i].Setup(this);
            }
        }

        int itemCount = _scrollRect.content.childCount;
        Selectable prevSelectable = null;
        for (int i = 0; i < itemCount; i++)
        {
            var selectable = _scrollRect.content.GetChild(i).GetComponentInChildren<Selectable>();

            var navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = prevSelectable;
            navigation.selectOnLeft = null;
            navigation.selectOnRight = null;

            if (i < itemCount - 1)
            {
                navigation.selectOnDown = _scrollRect.content.GetChild(i + 1).GetComponentInChildren<Selectable>();
            }

            selectable.navigation = navigation;

            prevSelectable = selectable;
        }

        RegisterModel(model);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);

        _scrollRect.verticalNormalizedPosition = 1.0f;

        for (int i = 0; i < _items.Length; i++)
        {
            _items[i].ShowDeselected();
        }

        if (_eventSystem.currentSelectedGameObject != _firstToSelect.gameObject)
        {
            _eventSystem.SetSelectedGameObject(_firstToSelect.gameObject);

            HandleItemSelect(_firstToSelect.GetComponent<GameSettingsViewItem>());
        }

        AssociatedButton.SetTabActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);

        AssociatedButton.SetTabActive(false);
    }

    public virtual void HandleItemSelect(GameSettingsViewItem item)
    {
        _view.HandleItemSelect(item);

        if (_inputType.Value != InputSource.SourceType.KeyboardMouse)
        {
            _scrollRect.ScrollToCenter(item.ParentRectTransform);
        }
        else
        {
            _scrollRect.ScrollToShow(item.ParentRectTransform);
        }
    }

    public virtual void HandleItemDeselect(GameSettingsViewItem item)
    {
        _view.HandleItemDeselect(item);
    }

    protected abstract void RegisterModel(GameSettings model);
}
