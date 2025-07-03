using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XInput;

public class InputDispatcher
{
    public ReadOnlyReactiveList<InputSource> PlayerInputSources { get; }
    private ReactiveList<InputSource> _playerInputSources;

    public ReadOnlyReactiveList<InputSource> AllInputSources { get; }
    private ReactiveList<InputSource> _allInputSources;

    public ReadOnlyReactiveProperty<InputSource.SourceType> UIInputType => _uiInputType;
    private ReactiveProperty<InputSource.SourceType> _uiInputType;

    public InputSource UIInput => _uiInputSource;
    private CommonUIInputSource _uiInputSource;

    private CommonGameplayInputSource _singleGameplayInputSource;

    private InputActionMap _gameplayMap;
    private InputActionMap _uiMap;
    private InputActionMap _uiDefaultMap;

    public InputDispatcher(InputActionMap gameplayMap, InputActionMap uiMap, InputActionMap uiDefaultMap)
    {
        _playerInputSources = new ReactiveList<InputSource>();
        PlayerInputSources = new ReadOnlyReactiveList<InputSource>(_playerInputSources);

        _allInputSources = new ReactiveList<InputSource>();
        AllInputSources = new ReadOnlyReactiveList<InputSource>(_allInputSources);

        _gameplayMap = gameplayMap;
        _uiMap = uiMap;
        _uiDefaultMap = uiDefaultMap;

        _uiInputType = new ReactiveProperty<InputSource.SourceType>(InputSource.SourceType.KeyboardMouse);

        _uiInputSource = new CommonUIInputSource(uiMap, uiDefaultMap, AllInputSources);

        _singleGameplayInputSource = new CommonGameplayInputSource(gameplayMap, this, _uiInputSource.UIInputProvider);

        for (int i = 0; i < InputSystem.devices.Count; i++)
        {
            var device = InputSystem.devices[i];

            _allInputSources.Add(new InputSource(device, gameplayMap, uiMap, uiDefaultMap));
        }

        _playerInputSources.OnItemAdd += (index, item) =>
        {
            if (_playerInputSources.Count > 1)
            {
                if (_playerInputSources.Contains(_singleGameplayInputSource))
                {
                    _playerInputSources.Remove(_singleGameplayInputSource);
                }
            }
        };

        _playerInputSources.OnItemRemove += (index, item) =>
        {
            if (_playerInputSources.Count == 0)
            {
                _playerInputSources.Add(_singleGameplayInputSource);
            }
        };

        _playerInputSources.OnBeforeClear += () =>
        {
            Timing.RunCoroutine(_DelayedAddSingleCommonGameplayProvider());
        };

        if (_playerInputSources.Count == 0)
        {
            _playerInputSources.Add(_singleGameplayInputSource);
        }

        InputSystem.onDeviceChange += OnDeviceChange;

        _gameplayMap.Enable();
        _uiMap.Enable();

        InputSystem.onEvent += OnInputEvent;

        Application.quitting += ApplicationQuitting;
    }

    public void VibrateAll()
    {
        Timing.RunCoroutine(_DoVibration(0.4f));
    }

    public void Tick()
    {

    }

    private void ApplicationQuitting()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (eventPtr.IsA<StateEvent>() || eventPtr.IsA<DeltaStateEvent>())
        {
            if (device != null && device.enabled)
            {
                if (device is Keyboard || device is Mouse)
                {
                    _uiInputType.Value = InputSource.SourceType.KeyboardMouse;
                }

                if (device is XInputController)
                {
                    _uiInputType.Value = InputSource.SourceType.XboxController;
                }

                if (device is DualShockGamepad)
                {
                    _uiInputType.Value = InputSource.SourceType.PsController;
                }
            }
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                _allInputSources.Add(new InputSource(device, _gameplayMap, _uiMap, _uiDefaultMap));
                Debug.Log($"Устройство подключено: {device.name}");
                break;

            case InputDeviceChange.Removed:
                for (int i = 0; i < _allInputSources.Count; i++)
                {
                    if (_allInputSources[i].Id == device.deviceId)
                    {
                        var source = _allInputSources[i];
                        _playerInputSources.Remove(source);
                        _allInputSources.RemoveAt(i);
                        source.Dispose();
                        break;
                    }
                }
                Debug.Log($"Устройство отключено: {device.name}");
                break;

            case InputDeviceChange.ConfigurationChanged:
            case InputDeviceChange.UsageChanged:
            case InputDeviceChange.Enabled:
            case InputDeviceChange.Disabled:
            case InputDeviceChange.SoftReset:
            case InputDeviceChange.HardReset:
                break;
        }
    }

    private IEnumerator<float> _DoVibration(float duration)
    {
        var devices = InputSystem.devices;

        for (int i = 0; i < devices.Count; i++)
        {
            if (devices[i] is Gamepad gamepad)
            {
                gamepad.SetMotorSpeeds(0.5f, 0.5f);
            }
        }

        yield return Timing.WaitForSeconds(duration);

        devices = InputSystem.devices;

        for (int i = 0; i < devices.Count; i++)
        {
            if (devices[i] is Gamepad gamepad)
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
            }
        }
    }

    private IEnumerator<float> _DelayedAddSingleCommonGameplayProvider()
    {
        yield return Timing.WaitForOneFrame;

        if (_allInputSources.Count == 0)
        {
            _allInputSources.Add(_singleGameplayInputSource);
        }
    }
}
