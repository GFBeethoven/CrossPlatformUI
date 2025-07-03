using Lean.Localization;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings
{
    private const string MasterVolumeParameter = "MasterVolume";
    private const string BgmVolumeParameter = "BGMVolume";
    private const string SfxVolumeParameter = "SFXVolume";

    public readonly Resolution[] AvailableResolutions;

    public ReadOnlyReactiveProperty<SystemLanguage> Language => _language;
    private ReactiveProperty<SystemLanguage> _language;

    public ReactiveProperty<GameDifficult> Difficult { get; }
    public ReactiveProperty<bool> AutoSave { get; }
    public ReactiveProperty<bool> Subtitles { get; }
    public ReactiveProperty<bool> SyncWithCloud { get; }

    public ReactiveProperty<float> MasterVolume { get; }
    public ReactiveProperty<float> BgmVolume { get; }
    public ReactiveProperty<float> SfxVolume { get; }

    public ReactiveProperty<FullScreenMode> FullScreenMode { get; }
    public ReactiveProperty<int> SelectedResolution { get; }

    public ReactiveProperty<float> HorizontalSensivity { get; }
    public ReactiveProperty<float> VerticalSensivity { get; }
    public ReactiveProperty<bool> InverseYAxis { get; }
    public ReactiveProperty<bool> Vibration { get; }

    private SettingsData _data;
    private ISaveLoadSystem _saveLoadSystem;
    private LeanLocalization _localization;
    private AudioMixer _audioMixer;

    public GameSettings(SettingsData data, LeanLocalization leanLocalization, AudioMixer audioMixer,
        ISaveLoadSystem saveLoadSystem)
    {
        AvailableResolutions = Screen.resolutions;

        _audioMixer = audioMixer;

        _localization = leanLocalization;

        _saveLoadSystem = saveLoadSystem;

        _data = data;

        _language = new ReactiveProperty<SystemLanguage>(data.Language);
        Difficult = new ReactiveProperty<GameDifficult>(data.Difficult);
        AutoSave = new ReactiveProperty<bool>(data.AutoSave);
        Subtitles = new ReactiveProperty<bool>(data.Subtitles);
        SyncWithCloud = new ReactiveProperty<bool>(data.SyncWithCloud);
        MasterVolume = new ReactiveProperty<float>(data.MasterVolume);
        BgmVolume = new ReactiveProperty<float>(data.BgmVolume);
        SfxVolume = new ReactiveProperty<float>(data.SfxVolume);
        FullScreenMode = new ReactiveProperty<FullScreenMode>(data.FullScreenMode);
        SelectedResolution = new ReactiveProperty<int>(GetResolutionIndex(data.Resolution));
        HorizontalSensivity = new ReactiveProperty<float>(data.HorizontalSensivity);
        VerticalSensivity = new ReactiveProperty<float>(data.VerticalSensivity);
        InverseYAxis = new ReactiveProperty<bool>(data.InverseYAxis);
        Vibration = new ReactiveProperty<bool>(data.Vibration);

        _language.Subscribe((v) =>
        {
            Debug.Log(v);
            _localization.SetCurrentLanguage(GetLanguageString(v));
            PlayerPrefs.SetInt("Language", (int)v);
            _data.Language = v;
        });

        Difficult.Subscribe((v) =>
        {
            _data.Difficult = v;
        });

        AutoSave.Subscribe((v) =>
        {
            _data.AutoSave = v;
        });

        Subtitles.Subscribe((v) =>
        {
            _data.Subtitles = v;
        });

        SyncWithCloud.Subscribe((v) =>
        {
            _data.SyncWithCloud = v;
        });

        MasterVolume.Subscribe((v) =>
        {
            _audioMixer.SetFloat(MasterVolumeParameter, ZeroOneRangeToDb(v));
            _data.MasterVolume = v;
        });

        BgmVolume.Subscribe((v) =>
        {
            _audioMixer.SetFloat(BgmVolumeParameter, ZeroOneRangeToDb(v));
            _data.MasterVolume = v;
        });

        SfxVolume.Subscribe((v) =>
        {
            _audioMixer.SetFloat(SfxVolumeParameter, ZeroOneRangeToDb(v));
            _data.SfxVolume = v;
        });

        FullScreenMode.Subscribe((v) =>
        {
            Screen.fullScreenMode = v;
            _data.FullScreenMode = v;
        });

        SelectedResolution.Subscribe((v) =>
        {
            Screen.SetResolution(AvailableResolutions[v].width, AvailableResolutions[v].height, FullScreenMode.Value);
            _data.Resolution = new Vector2Int(AvailableResolutions[v].width, AvailableResolutions[v].height);
        });

        HorizontalSensivity.Subscribe((v) =>
        {
            _data.HorizontalSensivity = v;
        });

        VerticalSensivity.Subscribe((v) =>
        {
            _data.VerticalSensivity = v;
        });

        InverseYAxis.Subscribe((v) =>
        {
            _data.InverseYAxis = v;
        });

        Vibration.Subscribe((v) =>
        {
            _data.Vibration = v;
        });

        Timing.RunCoroutine(_DelayedAudioVolumeSetup());
    }

    public void Save()
    {
        _saveLoadSystem.SaveGameSettings(_data);
    }

    public void SetLanguage(SystemLanguage language)
    {
        _language.Value = ValidateLanguage(language);
    }

    public void ResetToDefault()
    {
        _data = SettingsData.Default();

        Save();

        _language.Value = _data.Language;
        Difficult.Value = _data.Difficult;
        AutoSave.Value = _data.AutoSave;
        Subtitles.Value = _data.Subtitles;
        SyncWithCloud.Value = _data.SyncWithCloud;
        MasterVolume.Value = _data.MasterVolume;
        BgmVolume.Value = _data.BgmVolume;
        SfxVolume.Value = _data.SfxVolume;
        FullScreenMode.Value = _data.FullScreenMode;
        SelectedResolution.Value = GetResolutionIndex(_data.Resolution);
        HorizontalSensivity.Value = _data.HorizontalSensivity;
        VerticalSensivity.Value = _data.VerticalSensivity;
        InverseYAxis.Value = _data.InverseYAxis;
        Vibration.Value = _data.Vibration;
    }

    private static float ZeroOneRangeToDb(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    private IEnumerator<float> _DelayedAudioVolumeSetup()
    {
        yield return Timing.WaitForOneFrame;

        _audioMixer.SetFloat(MasterVolumeParameter, ZeroOneRangeToDb(MasterVolume.Value));
        _audioMixer.SetFloat(BgmVolumeParameter, ZeroOneRangeToDb(BgmVolume.Value));
        _audioMixer.SetFloat(SfxVolumeParameter, ZeroOneRangeToDb(SfxVolume.Value));
    }

    private int GetResolutionIndex(Vector2Int resolution)
    {
        Debug.Log(resolution);
        for (int i = 0; i < AvailableResolutions.Length; i++)
        {
            if (resolution.x == AvailableResolutions[i].width &&
                resolution.y == AvailableResolutions[i].height) return i;
        }

        return 0;
    }

    public static string GetLanguageString(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.English:
                return "English";
            case SystemLanguage.Russian:
                return "Russian";
        }

        return GetLanguageString(SystemLanguage.English);
    }

    private static SystemLanguage ValidateLanguage(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.English:
            case SystemLanguage.Russian:
                return language;
        }

        return SystemLanguage.English;
    }
}
