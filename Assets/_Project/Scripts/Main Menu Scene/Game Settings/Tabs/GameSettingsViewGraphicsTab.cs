using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsViewGraphicsTab : GameSettingsViewTab
{
    [SerializeField] private GraphicsSettingsElements _graphicsSettingsElements;

    protected override void RegisterModel(GameSettings model)
    {
        List<string> options = new List<string>();
        _graphicsSettingsElements.Resolutions.ClearOptions();
        for (int i = 0; i < model.AvailableResolutions.Length; i++)
        {
            options.Add($"{model.AvailableResolutions[i].width}x{model.AvailableResolutions[i].height}");
        }
        _graphicsSettingsElements.Resolutions.AddOptions(options);

        model.FullScreenMode.SubscribeWithEnableBinding((v) =>
        {
            _graphicsSettingsElements.FullscreenMode.Selector.SetOptionWithoutNotify(_graphicsSettingsElements.FullscreenMode.GetOption(v));
        }, this);
        model.SelectedResolution.SubscribeWithEnableBinding((v) =>
        {
            _graphicsSettingsElements.Resolutions.SetValueWithoutNotify(v);
        }, this);

        _graphicsSettingsElements.FullscreenMode.Selector.SubscribeOnValueChanged((o) =>
        {
            model.FullScreenMode.Value = _graphicsSettingsElements.FullscreenMode.FullScreenModes[o];
        });
        _graphicsSettingsElements.Resolutions.onValueChanged.AddListener((v) => model.SelectedResolution.Value = v);
    }

    [Serializable]
    public class FullscreenModeSelector
    {
        public LocalizedStringHorizontalSelector Selector;
        public FullScreenMode[] FullScreenModes;

        public int GetOption(FullScreenMode fullScreenMode)
        {
            for (int i = 0; i < FullScreenModes.Length; i++)
            {
                if (FullScreenModes[i] == fullScreenMode) return i;
            }

            return 0;
        }
    }

    [Serializable]
    public class GraphicsSettingsElements
    {
        public FullscreenModeSelector FullscreenMode;

        public TMP_Dropdown Resolutions;
    }
}
