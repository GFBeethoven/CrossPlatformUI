using DI;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameplayState : HFSMState
{
    private LoadGameplayView _view;

    private DIContainer _projectContainer;

    public LoadGameplayState(LoadGameplayView rootView, DIContainer projectContainer) : base(null, rootView)
    {
        _view = rootView;

        _projectContainer = projectContainer;
    }

    public override void Enter(StateEnterData enterData)
    {
        _view.Show();

        ReactivePropertyUnsubscriber[] dependencies = GameObject.FindObjectsByType<ReactivePropertyUnsubscriber>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var dependency in dependencies)
        {
            if (dependency != null)
            {
                GameObject.Destroy(dependency.gameObject);
            }
        }

        Timing.RunCoroutine(_LoadScene(enterData.As<GameplayEnterData>()));
    }

    public override void Exit()
    {
        _view.Hide();
    }

    public override void Update() { }

    public override void FixedUpdate() { }

    private IEnumerator<float> _LoadScene(GameplayEnterData enterData)
    {
        var operation = SceneManager.LoadSceneAsync(Scenes.Gameplay);

        while (operation.isDone == false)
        {
            yield return Timing.WaitForOneFrame;
        }

        yield return Timing.WaitForOneFrame;

        GameObject.FindFirstObjectByType<GameplayEntryPoint>().Run(_projectContainer, enterData);
    }
}
