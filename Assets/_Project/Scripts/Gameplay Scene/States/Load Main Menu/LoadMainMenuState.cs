using DI;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenuState : HFSMState
{
    private LoadMainMenuView _rootGameObject;

    private DIContainer _projectContainer;

    private GameSettings _gameSettings;

    private ISaveLoadSystem _saveLoadSystem;

    public LoadMainMenuState(LoadMainMenuView view, DIContainer projectContainer, 
        GameSettings gameSettings, ISaveLoadSystem saveLoadSystem) : base(null, view)
    {
        _rootGameObject = view;

        _saveLoadSystem = saveLoadSystem;

        _projectContainer = projectContainer;

        _gameSettings = gameSettings;
    }

    public override void Enter(StateEnterData enterData)
    {
        if (_gameSettings.AutoSave.Value && enterData is LoadMainMenuEnterData concreteData)
        {
            _saveLoadSystem.SaveGameplayState(concreteData.GameplayState, concreteData.ThumbnailPath);
        }

        _rootGameObject.Show();

        ReactivePropertyUnsubscriber[] dependencies = GameObject.FindObjectsByType<ReactivePropertyUnsubscriber>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var dependency in dependencies)
        {
            if (dependency != null)
            {
                GameObject.Destroy(dependency.gameObject);
            }
        }

        Timing.RunCoroutine(_LoadScene());
    }

    public override void Exit()
    {
        _rootGameObject.Hide();
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    private IEnumerator<float> _LoadScene()
    {
        var operation = SceneManager.LoadSceneAsync(Scenes.MainMenu);

        while (operation.isDone == false)
        {
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForOneFrame;

        GameObject.FindFirstObjectByType<MainMenuSceneEntryPoint>().Run(_projectContainer, MainMenuEnterData.MainMenuEnterDataEmpty);
    }
}
