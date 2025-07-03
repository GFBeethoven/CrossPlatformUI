using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsViewControlTab : GameSettingsViewTab
{
    public event Action VibrationIsOn;

    [SerializeField] private ControlSettingsElements _controlSettingsElements;

    protected override void RegisterModel(GameSettings model)
    {
        model.HorizontalSensivity.SubscribeWithEnableBinding((v) => _controlSettingsElements.HorizontalSensivity.SetValueWithoutNotify(v), this);
        model.VerticalSensivity.SubscribeWithEnableBinding((v) => _controlSettingsElements.VerticalSensivity.SetValueWithoutNotify(v), this);
        model.InverseYAxis.SubscribeWithEnableBinding((v) => _controlSettingsElements.InverseYAxis.SetIsOnWithoutNotify(v), this);
        model.Vibration.SubscribeWithEnableBinding((v) => _controlSettingsElements.Vibration.SetIsOnWithoutNotify(v), this);

        _controlSettingsElements.HorizontalSensivity.onValueChanged.AddListener((v) => model.HorizontalSensivity.Value = v);
        _controlSettingsElements.VerticalSensivity.onValueChanged.AddListener((v) => model.VerticalSensivity.Value = v);
        _controlSettingsElements.InverseYAxis.onValueChanged.AddListener((o) => model.InverseYAxis.Value = o);
        _controlSettingsElements.Vibration.onValueChanged.AddListener((o) =>
        {
            model.Vibration.Value = o;

            if (model.Vibration.Value)
            {
                VibrationIsOn?.Invoke();
            }
        });
    }

    [Serializable]
    public class ControlSettingsElements
    {
        public Slider HorizontalSensivity;

        public Slider VerticalSensivity;

        public Toggle InverseYAxis;

        public Toggle Vibration;
    }
}
