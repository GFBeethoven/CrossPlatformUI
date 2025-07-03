using DI;
using Lean.Localization;
using MEC;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetermineLanguageOperation : ILoadingOperation
{
    public LoadingStatus Status { get; private set; } = LoadingStatus.Running;

    public float Progress { get; private set; } = 0.0f;

    public string DescriptionKey => "";

    public string FailureMessageKey { get; private set; } = null;

    private DIContainer _projectContainer;

    public DetermineLanguageOperation(DIContainer projectContainer)
    {
        _projectContainer = projectContainer;
    }

    public void Launch()
    {
        SystemLanguage language = (SystemLanguage)PlayerPrefs.GetInt("Language", (int)Application.systemLanguage);

        _projectContainer.Resolve<LeanLocalization>().SetCurrentLanguage(GameSettings.GetLanguageString(language));

        Progress = 1.0f;

        Status = LoadingStatus.Success;
    }

    private IEnumerator<float> _Operation(ISaveLoadSystem saveLoadSystem)
    {
        Status = LoadingStatus.Running;

#if UNITY_EDITOR
        Progress = 0.5f;

        Debug.Log($"Loaded: {saveLoadSystem.GetAllSavedGameplayStates().Length}");

        yield return Timing.WaitForOneFrame;
#else
        Progress = 0.5f;

        yield return Timing.WaitForOneFrame;
#endif

        Status = LoadingStatus.Success;
    }
}
