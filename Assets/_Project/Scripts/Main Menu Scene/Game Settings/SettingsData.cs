using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SettingsData
{
    public SystemLanguage Language;
    public GameDifficult Difficult;
    public bool AutoSave;
    public bool Subtitles;
    public bool SyncWithCloud;

    public float MasterVolume;
    public float BgmVolume;
    public float SfxVolume;

    public FullScreenMode FullScreenMode;
    public Vector2Int Resolution;

    public float HorizontalSensivity;
    public float VerticalSensivity;

    public bool InverseYAxis;

    public bool Vibration;

    public string ControlBinding;

    public static SettingsData Default()
    {
        return new SettingsData()
        {
            Language = Application.systemLanguage,
            Difficult = GameDifficult.Medium,
            AutoSave = true,
            Subtitles = false,
            SyncWithCloud = false,
            MasterVolume = 0.5f,
            BgmVolume = 0.6f,
            SfxVolume = 0.35f,
            FullScreenMode = FullScreenMode.FullScreenWindow,
            Resolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height),
            HorizontalSensivity = 0.5f,
            VerticalSensivity = 0.5f,
            InverseYAxis = false,
            ControlBinding = "",
            Vibration = true
        };
    }
}
