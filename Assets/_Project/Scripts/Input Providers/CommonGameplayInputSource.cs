using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommonGameplayInputSource : InputSource
{
    public override SourceType Type => SourceType.KeyboardMouse;

    public override IUIInputProvider UIInputProvider => _uiProvider;
    private IUIInputProvider _uiProvider;

    public override IGameplayInputProvider GameplayInputProvider => _inputProvider;
    private IGameplayInputProvider _inputProvider;

    private InputDispatcher _dispatcher;

    public CommonGameplayInputSource(InputActionMap gameplayMap, InputDispatcher dispatcher, 
        IUIInputProvider commonUIProvider) : base()
    {
        _inputProvider = new CommonGameplayInputProvider(gameplayMap);
        _dispatcher = dispatcher;
        _uiProvider = commonUIProvider;
    }

    public override void TryVibrate()
    {
        _dispatcher.VibrateAll();
    }

    private class CommonGameplayInputProvider : IGameplayInputProvider
    {
        private InputAction _crossMoveAction;
        private InputAction _shotAction;
        private InputAction _pauseAction;

        public CommonGameplayInputProvider(InputActionMap map)
        {
            _crossMoveAction = map.FindAction("MoveCross");
            _shotAction = map.FindAction("Shot");
            _pauseAction = map.FindAction("Pause");
        }

        public Vector2 ControlAxis
        {
            get
            {
                return _crossMoveAction.ReadValue<Vector2>();
            }
        }

        public bool IsShootingUp
        {
            get
            {
                return _shotAction.WasReleasedThisFrame();
            }
        }

        public bool IsShootingDown
        {
            get
            {
                return _shotAction.WasPressedThisFrame();
            }
        }

        public bool IsPauseRequested
        {
            get
            {
                return _pauseAction.WasPressedThisFrame();
            }
        }

        public void Dispose()
        {
            _crossMoveAction = null;
            _pauseAction = null;
            _shotAction = null;
        }
    }
}
