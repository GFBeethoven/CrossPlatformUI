using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsViewGeneralTab : GameSettingsViewTab
{
    public event Action DeleteSavesRequested;
    public event Action CreditsRequested;

    [SerializeField] private GenenalSettingsElements _genenalSettingsElements;

    protected override void RegisterModel(GameSettings model)
    {
        model.Language.SubscribeWithEnableBinding((v) =>
        {
            _genenalSettingsElements.Language.Selector.SetOptionWithoutNotify(_genenalSettingsElements.Language.GetOption(v));
        }, this);
        model.Difficult.SubscribeWithEnableBinding((v) =>
        {
            _genenalSettingsElements.GameDifficult.Selector.SetOptionWithoutNotify(_genenalSettingsElements.GameDifficult.GetOption(v));
        }, this);
        model.AutoSave.SubscribeWithEnableBinding((v) => _genenalSettingsElements.AutoSave.SetIsOnWithoutNotify(v), this);
        model.SyncWithCloud.SubscribeWithEnableBinding((v) => _genenalSettingsElements.SyncWithCloud.SetIsOnWithoutNotify(v), this);
        model.Subtitles.SubscribeWithEnableBinding((v) => _genenalSettingsElements.Subtitles.SetIsOnWithoutNotify(v), this);

        _genenalSettingsElements.Language.Selector.SubscribeOnValueChanged((o) =>
        {
            model.SetLanguage(_genenalSettingsElements.Language.LanguageOptions[o]);
        });
        _genenalSettingsElements.GameDifficult.Selector.SubscribeOnValueChanged((o) =>
        {
            model.Difficult.Value = _genenalSettingsElements.GameDifficult.DifficultOptions[o];
        });
        _genenalSettingsElements.AutoSave.onValueChanged.AddListener((o) => model.AutoSave.Value = o);
        _genenalSettingsElements.Subtitles.onValueChanged.AddListener((o) => model.Subtitles.Value = o);
        _genenalSettingsElements.SyncWithCloud.onValueChanged.AddListener((o) => model.SyncWithCloud.Value = o);

        _genenalSettingsElements.Credits.onClick.AddListener(() => CreditsRequested?.Invoke());
        _genenalSettingsElements.DeleteSaves.onClick.AddListener(() => DeleteSavesRequested?.Invoke());
    }

    public void SetupSavesCountReact(ReadOnlyReactiveProperty<int> savesCount)
    {
        savesCount.SubscribeWithEnableBinding((c) =>
        {
            _genenalSettingsElements.DeleteSaves.interactable = c > 0;
        }, _genenalSettingsElements.DeleteSaves);
    }

    [Serializable]
    public class LanguageSelector
    {
        public LocalizedStringHorizontalSelector Selector;
        public SystemLanguage[] LanguageOptions;

        public int GetOption(SystemLanguage language)
        {
            for (int i = 0; i < LanguageOptions.Length; i++)
            {
                if (LanguageOptions[i] == language) return i;
            }

            return 0;
        }
    }

    [Serializable]
    public class GameDifficultSelector
    {
        public LocalizedStringHorizontalSelector Selector;
        public GameDifficult[] DifficultOptions;

        public int GetOption(GameDifficult difficult)
        {
            for (int i = 0; i < DifficultOptions.Length; i++)
            {
                if (DifficultOptions[i] == difficult) return i;
            }

            return 0;
        }
    }

    [Serializable]
    public class GenenalSettingsElements
    {
        public LanguageSelector Language;

        public GameDifficultSelector GameDifficult;

        public Toggle AutoSave;

        public Toggle SyncWithCloud;

        public Toggle Subtitles;

        public Button DeleteSaves;

        public Button Credits;
    }
}
