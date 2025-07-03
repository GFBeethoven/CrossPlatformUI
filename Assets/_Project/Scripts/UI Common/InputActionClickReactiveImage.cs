using MEC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(Image))]
public class InputActionClickReactiveImage : CommonUIInputSourceTypeDependence
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

        _observableAction.performed += ActionPerformed;

        _observableAction.Enable();

        SetState(0.0f);

        SetForwardFill();
    }

    private void OnDisable()
    {
        _observableAction.performed -= ActionPerformed;

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

    private void SetBackwardFill()
    {
        _pressedImage.type = Image.Type.Filled;
        _pressedImage.fillMethod = Image.FillMethod.Radial180;
        _pressedImage.fillOrigin = (int)Image.Origin180.Top;
        _pressedImage.fillClockwise = true;

        _cachedImage.type = Image.Type.Filled;
        _cachedImage.fillMethod = Image.FillMethod.Radial180;
        _cachedImage.fillOrigin = (int)Image.Origin180.Top;
        _cachedImage.fillClockwise = false;
    }

    private void ActionPerformed(InputAction.CallbackContext context)
    {
        if (_reactCoroutine.IsValid) return;

        _reactCoroutine = Timing.RunCoroutine(_ReactAnimation());
    }

    private void SetState(float i)
    {
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

    private IEnumerator<float> _ReactAnimation()
    {
        const float Duration = 0.2f;

        SetForwardFill();

        SetState(0.0f);

        for (float i = 0.0f; i < 1.0f; i += Timing.DeltaTime / Duration)
        {
            float t = Easing.InQuad(i);

            SetState(t);

            yield return Timing.WaitForOneFrame;
        }

        SetState(1.0f);

        SetBackwardFill();

        for (float i = 1.0f; i > 0.0f; i -= Timing.DeltaTime / Duration)
        {
            float t = Easing.OutQuad(i);

            SetState(t);

            yield return Timing.WaitForOneFrame;
        }

        SetState(0.0f);

        SetForwardFill();

        _reactCoroutine = new CoroutineHandle();
    }
}
