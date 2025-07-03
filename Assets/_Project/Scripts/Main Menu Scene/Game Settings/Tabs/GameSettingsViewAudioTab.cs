using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsViewAudioTab : GameSettingsViewTab
{
    [SerializeField] private AudioSettingsElements _audioSettingsElements;

    protected override void RegisterModel(GameSettings model)
    {
        model.MasterVolume.SubscribeWithEnableBinding((v) => _audioSettingsElements.MasterVolume.SetValueWithoutNotify(v), this);
        model.BgmVolume.SubscribeWithEnableBinding((v) => _audioSettingsElements.BGMVolume.SetValueWithoutNotify(v), this);
        model.SfxVolume.SubscribeWithEnableBinding((v) => _audioSettingsElements.SFXVolume.SetValueWithoutNotify(v), this);

        _audioSettingsElements.MasterVolume.onValueChanged.AddListener((v) => model.MasterVolume.Value = v);
        _audioSettingsElements.BGMVolume.onValueChanged.AddListener((v) => model.BgmVolume.Value = v);
        _audioSettingsElements.SFXVolume.onValueChanged.AddListener((v) => model.SfxVolume.Value = v);
    }

    [Serializable]
    public class AudioSettingsElements
    {
        public Slider MasterVolume;

        public Slider BGMVolume;

        public Slider SFXVolume;
    }
}
