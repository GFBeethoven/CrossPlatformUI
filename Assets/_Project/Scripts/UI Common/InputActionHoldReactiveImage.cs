using MEC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(Image))]
public class InputActionHoldReactiveImage : CommonUIInputSourceTypeDependence
{
    [SerializeField] private InputAction _observableAction;

    [SerializeField] private Image _pressedImage;

    private Dictionary<InputSource.SourceType, string> _controlPathByTypeMap = new();

    private Image _cachedImage;
    private Image Image
    {
        get
        {
            if (_cachedImage != null) return _cachedImage;

            _cachedImage = GetComponent<Image>();

            return _cachedImage;
        }
    }

    private CoroutineHandle _reactCoroutine;

    private bool _isInitialized;

    private float _state = 0.0f;

    public override void InputTypeChanged(InputSource.SourceType type)
    {
        string path = null;

        if (_controlPathByTypeMap.TryGetValue(type, out string value))
        {
            path = value;
        }
        else
        {
            path = ExtractControlPathAssociatedWithSourceType(type);
        }

        if (path != null && InputSourceIcons.Instance != null)
        {
            var icons = InputSourceIcons.Instance.GetIconByControlPath(path);

            if (icons.Item1 != null)
            {
                Image.sprite = icons.Item1;
                _pressedImage.sprite = icons.Item2;
            }
        }
    }

    private void OnEnable()
    {
        if (!_isInitialized)
        {
            Initialize();
        }

        _observableAction.started += ActionStarted;
        _observableAction.canceled += ActionCanceled;

        _observableAction.Enable();

        SetState(0.0f);

        SetForwardFill();
    }

    private void OnDisable()
    {
        _observableAction.started -= ActionStarted;
        _observableAction.canceled -= ActionCanceled;

        _observableAction.Disable();

        if (_reactCoroutine.IsValid)
        {
            Timing.KillCoroutines(_reactCoroutine);
        }
    }

    private void Initialize()
    {
        if (_isInitialized) return;

        _cachedImage = GetComponent<Image>();

        SetForwardFill();

        _controlPathByTypeMap[InputSource.SourceType.KeyboardMouse] =
            ExtractControlPathAssociatedWithSourceType(InputSource.SourceType.KeyboardMouse);
        _controlPathByTypeMap[InputSource.SourceType.XboxController] =
            ExtractControlPathAssociatedWithSourceType(InputSource.SourceType.XboxController);
        _controlPathByTypeMap[InputSource.SourceType.PsController] =
            ExtractControlPathAssociatedWithSourceType(InputSource.SourceType.PsController);

        _isInitialized = true;
    }

    private void SetForwardFill()
    {
        _pressedImage.type = Image.Type.Filled;
        _pressedImage.fillMethod = Image.FillMethod.Radial180;
        _pressedImage.fillOrigin = (int)Image.Origin180.Top;
        _pressedImage.fillClockwise = false;

        _cachedImage.type = Image.Type.Filled;
        _cachedImage.fillMethod = Image.FillMethod.Radial180;
        _cachedImage.fillOrigin = (int)Image.Origin180.Top;
        _cachedImage.fillClockwise = true;
    }

    private void ActionCanceled(InputAction.CallbackContext obj)
    {
        if (_reactCoroutine.IsValid)
        {
            Timing.KillCoroutines(_reactCoroutine);
        }

        _reactCoroutine = Timing.RunCoroutine(_ReactOut());
    }

    private void ActionStarted(InputAction.CallbackContext obj)
    {
        if (_reactCoroutine.IsValid)
        {
            Timing.KillCoroutines(_reactCoroutine);
        }

        _reactCoroutine = Timing.RunCoroutine(_ReactIn());
    }

    private void SetState(float i)
    {
        _state = i;

        Image.fillAmount = 1.0f - i;
        _pressedImage.fillAmount = i;
    }

    private string ExtractControlPathAssociatedWithSourceType(InputSource.SourceType type)
    {
        string mask = null;

        switch (type)
        {
            case InputSource.SourceType.KeyboardMouse:
                mask = "<Keyboard>";
                break;
            case InputSource.SourceType.XboxController:
                mask = "<XInputController>";
                break;
            case InputSource.SourceType.PsController:
                mask = "<DualShockGamepad>";
                break;
        }

        for (int i = 0; i < _observableAction.bindings.Count; i++)
        {
            if (_observableAction.bindings[i].path.StartsWith(mask))
            {
                return _observableAction.bindings[i].path;
            }
        }

        return null;
    }

    private IEnumerator<float> _ReactIn()
    {
        const float Duration = 0.2f;

        float startState = _state;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            float t = Mathf.Lerp(startState, 1.0f, Easing.InQuad(i));

            SetState(t);

            yield return Timing.WaitForOneFrame;
        }

        SetState(1.0f);
    }

    private IEnumerator<float> _ReactOut()
    {
        const float Duration = 0.2f;

        float startState = _state;

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            float t = Mathf.Lerp(startState, 0.0f, Easing.OutQuad(i));

            SetState(t);

            yield return Timing.WaitForOneFrame;
        }

        SetState(0.0f);
    }
}
