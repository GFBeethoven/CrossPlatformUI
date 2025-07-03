using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class InputSource : IDisposable
{
    public int Id { get; }

    public virtual SourceType Type { get; }

    public virtual IGameplayInputProvider GameplayInputProvider { get; private set; }

    public virtual IUIInputProvider UIInputProvider { get; private set; }

    public InputSource(InputDevice device, InputActionMap gameplayMap, InputActionMap uiMap, InputActionMap uiDefaultMap)
    {
        Id = device.deviceId;

        Type = GetTypeByDevice(device);

        GameplayInputProvider = new DefaultGameplayInputProvider(Id, gameplayMap);
        
        UIInputProvider = new DefaultUIInputProvider(device, uiMap, uiDefaultMap);
    }

    public InputSource()
    {
        Id = 0;
    }

    public virtual void TryVibrate()
    {
        var devices = InputSystem.devices;

        for (int i = 0; i < devices.Count; i++)
        {
            if (devices[i].deviceId == Id &&
                devices[i] is Gamepad gamepad)
            {
                Timing.RunCoroutine(_DoVibrate(gamepad, 0.5f));
            }
        }
    }

    public virtual void Dispose()
    {
        GameplayInputProvider?.Dispose();
        UIInputProvider?.Dispose();

        GameplayInputProvider = null;
        UIInputProvider = null;
    }

    private static SourceType GetTypeByDevice(InputDevice device)
    {
        if (device is XInputController) return SourceType.XboxController;
        if (device is DualShockGamepad) return SourceType.PsController;

        return SourceType.KeyboardMouse;
    }

    private static IEnumerator<float> _DoVibrate(Gamepad gamepad, float duration)
    {
        gamepad.SetMotorSpeeds(0.5f, 0.5f);

        yield return Timing.WaitForSeconds(duration);

        gamepad.SetMotorSpeeds(0.0f, 0.0f);
    }

    public enum SourceType
    {
        KeyboardMouse,
        XboxController,
        PsController
    }

    private class DefaultInputProvider
    {
        private int _deviceId;

        public DefaultInputProvider(int deviceId)
        {
            _deviceId = deviceId;
        }

        protected InputControl GetAssociatedFirstControl(InputAction action)
        {
            var controls = action.controls;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].device.deviceId == _deviceId) return controls[i];
            }

            return null;
        }

        protected Vector2 GetVector2ControlValue(InputAction action)
        {
            var controls = action.controls;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].device.deviceId == _deviceId)
                {
                    if (controls[i] is Vector2Control control)
                    {
                        return control.ReadValue();
                    }
                }
            }

            return Vector2.zero;
        }

        protected bool WasButtonPressed(InputAction action)
        {
            var controls = action.controls;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].device.deviceId == _deviceId)
                {
                    if (controls[i] is ButtonControl control)
                    {
                        if (control.wasPressedThisFrame)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected bool WasButtonReleased(InputAction action)
        {
            var controls = action.controls;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].device.deviceId == _deviceId)
                {
                    if (controls[i] is ButtonControl control)
                    {
                        if (control.wasReleasedThisFrame)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected bool IsButtonPressed(InputAction action)
        {
            var controls = action.controls;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].device.deviceId == _deviceId)
                {
                    if (controls[i] is ButtonControl control)
                    {
                        if (control.isPressed)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    private class DefaultGameplayInputProvider : DefaultInputProvider, IGameplayInputProvider
    {
        private InputAction _crossMoveAction;
        private InputAction _shotAction;
        private InputAction _pauseAction;

        public DefaultGameplayInputProvider(int deviceId, InputActionMap map) : base(deviceId)
        {
            _crossMoveAction = map.FindAction("MoveCross");
            _shotAction = map.FindAction("Shot");
            _pauseAction = map.FindAction("Pause");
        }

        public Vector2 ControlAxis
        {
            get
            {
                return GetVector2ControlValue(_crossMoveAction);
            }
        }

        public bool IsShootingUp
        {
            get
            {
                return WasButtonReleased(_shotAction);
            }
        }

        public bool IsShootingDown
        {
            get
            {
                return WasButtonPressed(_shotAction);
            }
        }

        public bool IsPauseRequested
        {
            get
            {
                return WasButtonPressed(_pauseAction);
            }
        }

        public void Dispose()
        {
            _crossMoveAction = null;
            _pauseAction = null;
            _shotAction = null;
        }
    }

    private class DefaultUIInputProvider : DefaultInputProvider, IUIInputProvider
    {
        private InputAction _prevTab;
        private InputAction _nextTab;
        private InputAction _resetToDefault;
        private InputAction _deleteSave;
        private InputAction _anyKey;
        private InputAction _back;
        private InputAction _confirm;
        private InputAction _speedUp;

        private InputDevice _device;

        public DefaultUIInputProvider(InputDevice device, InputActionMap map, InputActionMap defaultUIMap) : base(device.deviceId)
        {
            _prevTab = GetAction(map, defaultUIMap, "Previous Tab");
            _nextTab = GetAction(map, defaultUIMap, "Next Tab");
            _anyKey = GetAction(map, defaultUIMap, "Any Key");
            _resetToDefault = GetAction(map, defaultUIMap, "Reset To Default Settings");
            _deleteSave = GetAction(map, defaultUIMap, "Delete Save");
            _back = GetAction(map, defaultUIMap, "Back");
            _confirm = GetAction(map, defaultUIMap, "Confirm");
            _speedUp = GetAction(map, defaultUIMap, "Speed Up Credits");

            _device = device;

            InputAction GetAction(InputActionMap map, InputActionMap defaultUIMap, string name)
            {
                var action = map.FindAction(name);

                if (action != null) return action;

                return defaultUIMap.FindAction(name);
            }
        }

        public bool IsAnyActionNow
        {
            get
            {
                if (_device != null)
                {
                    var controls = _device.allControls;

                    for (int i = 0; i < controls.Count; i++)
                    {
                        var control = controls[i];

                        if (control is ButtonControl button)
                        {
                            if (button.wasPressedThisFrame || button.isPressed)
                                return true;
                        }
                        else if (control is AxisControl axis)
                        {
                            if (Mathf.Abs(axis.ReadValue()) > 0.1f)
                                return true;
                        }
                        else if (control is Vector2Control vec2)
                        {
                            if (vec2.ReadValue().magnitude > 0.1f)
                                return true;
                        }
                    }
                }

                return false;
            }
        }

        public bool IsAnyButtonDown
        {
            get
            {
                return WasButtonPressed(_anyKey);
            }
        }

        public bool IsBackButtonDown
        {
            get
            {
                return WasButtonPressed(_back);
            }
        }

        public bool IsToLeftTabButtonDown
        {
            get
            {
                return WasButtonPressed(_prevTab);
            }
        }

        public bool IsToRightTabButtonDown
        {
            get
            {
                return WasButtonPressed(_nextTab);
            }
        }

        public bool IsConfirmButtonDown
        {
            get
            {
                return WasButtonPressed(_confirm);
            }
        }

        public bool IsResetToDefaultButtonDown
        {
            get
            {
                return WasButtonPressed(_resetToDefault);
            }
        }

        public bool IsDeleteSaveButtonDown
        {
            get
            {
                return WasButtonPressed(_deleteSave);
            }
        }

        public bool SpeedUpCreditsButton
        {
            get
            {
                return IsButtonPressed(_speedUp);
            }
        }

        public void Dispose()
        {
            _prevTab = null;
            _nextTab = null;
            _resetToDefault = null;
            _deleteSave = null;
            _anyKey = null;
            _confirm = null;
            _back = null;

            _device = null;
        }
    }
}
