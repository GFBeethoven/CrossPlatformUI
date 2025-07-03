using DI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoadGameStateOperation : ILoadingOperation
{
    public LoadingStatus Status { get; private set; } = LoadingStatus.Running;

    public float Progress { get; private set; } = 0.0f;

    public string DescriptionKey => "LoadGameState";

    public string FailureMessageKey { get; private set; } = null;


    private DIContainer _projectContainer;

    public LoadGameStateOperation(DIContainer projectContainer)
    {
        _projectContainer = projectContainer;
    }

    public void Launch()
    {
        Timing.RunCoroutine(_Operation(_projectContainer.Resolve<ISaveLoadSystem>()));
    }

    private IEnumerator<float> _Operation(ISaveLoadSystem saveLoadSystem)
    {
        Status = LoadingStatus.Running;

#if UNITY_EDITOR
        Progress = 0.5f;

        saveLoadSystem.GetAllSavedGameplayStates();

        yield return Timing.WaitForSeconds(0.5f);
#else
        Progress = 0.5f;

        yield return Timing.WaitForOneFrame;
#endif

        Status = LoadingStatus.Success;
    }
}
