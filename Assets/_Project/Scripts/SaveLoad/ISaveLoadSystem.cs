using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveLoadSystem
{
    public ReadOnlyReactiveProperty<int> SavedStateCount { get; }

    public void SaveGameplayState(GameplayStateData gameplayStateData, string thumbnailPath = null);

    public void DeleteGameplayState(GameplayStateData gameplayStateData);

    public void DeleteAllGameplayStates();

    public (GameplayStateData, Texture2D)[] GetAllSavedGameplayStates();

    public void SaveGameSettings(SettingsData settingsData);

    public SettingsData LoadGameSettings();
}
