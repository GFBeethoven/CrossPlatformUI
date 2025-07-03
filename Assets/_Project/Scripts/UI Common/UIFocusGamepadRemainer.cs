using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIFocusGamepadRemainer : MonoBehaviour
{
    private GameObject _prevSelectedObject;

    private ReadOnlyReactiveProperty<InputSource.SourceType> _currentType;

    public void Initialize(InputDispatcher inputDispatcher)
    {
        _currentType = inputDispatcher.UIInputType;

        _currentType.Subscribe(InputTypeChanged);
    }

    private void InputTypeChanged(InputSource.SourceType type)
    {
        gameObject.SetActive(type != InputSource.SourceType.KeyboardMouse);
    }

    private void LateUpdate()
    {
        var eventSystem = EventSystem.current;

        if (eventSystem.currentSelectedGameObject != null)
        {
            _prevSelectedObject = eventSystem.currentSelectedGameObject;
        }

        if (eventSystem.currentSelectedGameObject == null)
        {
            if (!eventSystem.alreadySelecting)
            {
                if (_prevSelectedObject == null)
                {
                    _prevSelectedObject = FindFirstObjectByType<Selectable>(FindObjectsInactive.Exclude)?.gameObject;
                }

                if (_prevSelectedObject != null)
                {
                    eventSystem.SetSelectedGameObject(_prevSelectedObject);
                }
            }
        }
    }
}
