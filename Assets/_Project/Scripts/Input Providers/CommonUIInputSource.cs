using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CommonUIInputSource : InputSource
{
    public override SourceType Type => _uiProvider.PrevActiveSourceType;

    public override IGameplayInputProvider GameplayInputProvider => null;

    public override IUIInputProvider UIInputProvider => _uiProvider;

    private CommonUIInputProvider _uiProvider;

    public CommonUIInputSource(InputActionMap uiMap, InputActionMap defaultUiMap, 
        ReadOnlyReactiveList<InputSource> allSources) : base()
    {
        _uiProvider = new CommonUIInputProvider(uiMap, defaultUiMap, allSources);
    }

    private class CommonUIInputProvider : IUIInputProvider
    {
        public SourceType PrevActiveSourceType { get; private set; }

        private InputAction _prevTab;
        private InputAction _nextTab;
        private InputAction _resetToDefault;
        private InputAction _deleteSave;
        private InputAction _anyKey;
        private InputAction _back;
        private InputAction _confirm;
        private InputAction _speedUp;

        private ReadOnlyReactiveList<InputSource> _allSources;

        public CommonUIInputProvider(InputActionMap map, InputActionMap defaultUIMap, ReadOnlyReactiveList<InputSource> allSources)
        {
            _allSources = allSources;

            _prevTab = GetAction(map, defaultUIMap, "Previous Tab");
            _nextTab = GetAction(map, defaultUIMap, "Next Tab");
            _anyKey = GetAction(map, defaultUIMap, "Any Key");
            _resetToDefault = GetAction(map, defaultUIMap, "Reset To Default Settings");
            _deleteSave = GetAction(map, defaultUIMap, "Delete Save");
            _back = GetAction(map, defaultUIMap, "Cancel");
            _confirm = GetAction(map, defaultUIMap, "Submit");
            _speedUp = GetAction(map, defaultUIMap, "Speed Up Credits");

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
                for (int i = 0; i < _allSources.Count; i++)
                {
                    if (_allSources[i].UIInputProvider.IsAnyActionNow) return true;
                }

                return false;
            }
        }

        public bool IsAnyButtonDown
        {
            get
            {
                return _anyKey.WasPressedThisFrame();
            }
        }

        public bool IsBackButtonDown
        {
            get
            {
                return _back.WasPressedThisFrame();
            }
        }

        public bool IsToLeftTabButtonDown
        {
            get
            {
                return _prevTab.WasPressedThisFrame();
            }
        }

        public bool IsToRightTabButtonDown
        {
            get
            {
                return _nextTab.WasPressedThisFrame();
            }
        }

        public bool IsConfirmButtonDown
        {
            get
            {
                return _confirm.WasPressedThisFrame();
            }
        }

        public bool IsResetToDefaultButtonDown
        {
            get
            {
                return _resetToDefault.WasPressedThisFrame();
            }
        }

        public bool IsDeleteSaveButtonDown
        {
            get
            {
                return _deleteSave.WasPressedThisFrame();
            }
        }

        public bool SpeedUpCreditsButton
        {
            get
            {
                return _speedUp.IsPressed();
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
        }
    }
}
