using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMenuModel
{
    public ReadOnlyReactiveList<(GameplayStateData, Texture2D)> SavedStates { get; }

    private ReactiveList<(GameplayStateData, Texture2D)> _savedStates = new();

    private ISaveLoadSystem _saveLoadSystem;

    public LoadMenuModel(ISaveLoadSystem saveLoadSystem)
    {
        _saveLoadSystem = saveLoadSystem;

        SavedStates = new ReadOnlyReactiveList<(GameplayStateData, Texture2D)>(_savedStates);
    }

    public void Refresh()
    {
        _savedStates.Clear();

        var allSaves = _saveLoadSystem.GetAllSavedGameplayStates();

        for (int i = 0; i < allSaves.Length; i++)
        {
            _savedStates.Add(allSaves[i]);
        }
    }

    public void DeleteState(GameplayStateData state)
    {
        _saveLoadSystem.DeleteGameplayState(state);

        int index = -1;

        for (int i = 0; i < _savedStates.Count; i++)
        {
            if (_savedStates[i].Item1 == state)
            {
                index = i;

                break;
            }
        }

        if (index != -1)
        {
            _savedStates.RemoveAt(index);
        }
    }
}
