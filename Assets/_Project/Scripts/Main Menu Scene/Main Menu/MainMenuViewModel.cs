using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuViewModel
{
    public event Action StartGameRequested;
    public event Action LoadMenuRequested;
    public event Action SettingsRequested;
    public event Action GameExitRequested;

    public ReadOnlyReactiveProperty<int> SavedStatesCount { get; }

    public MainMenuViewModel(ReadOnlyReactiveProperty<int> savedStatesCount)
    {
        SavedStatesCount = savedStatesCount;
    }

    public void RequestStartGame()
    {
        StartGameRequested?.Invoke();
    }

    public void RequestLoadMenu()
    {
        LoadMenuRequested?.Invoke();    
    }

    public void RequestSettings()
    {
        SettingsRequested?.Invoke();    
    }

    public void RequestGameExit()
    {
        GameExitRequested?.Invoke();
    }
}
