using DI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SceneEntryPoint : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    protected DIContainer ProjectContainer { get; private set; }

    private DIContainer _sceneContainer;

    public void Run(DIContainer projectContainer, StateEnterData stateEnterData)
    {
        ProjectContainer = projectContainer;

        Run(stateEnterData, out _sceneContainer);

        SceneManager.sceneUnloaded += ActiveSceneChanged;
    }

    private void ActiveSceneChanged(Scene scene)
    {
        if (scene.name == _sceneName)
        {
            SceneManager.sceneUnloaded -= ActiveSceneChanged;

            DisposeSceneDependencies();
        }
    }

    public abstract void Run(StateEnterData enterData, out DIContainer sceneContainer);

    public virtual void DisposeSceneDependencies()
    {
        _sceneContainer.Dispose();

        _sceneContainer = null;
    }
}
