using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DI;
using MEC;

public class GameEntryPoint
{
    private static GameEntryPoint _instance = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void OnGameLoad()
    {
        if (_instance != null) return;

        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _instance = new GameEntryPoint();
        _instance.RunGame();
    }

    private GameEntryPoint() { }

    private void RunGame()
    {
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name != Scenes.Boot)
        {
            SceneManager.LoadScene(Scenes.Boot);
        }
#endif

        DIContainer projectContainter = new DIContainer();

        Timing.RunCoroutine(_RunGame(projectContainter));
    }

    private IEnumerator<float> _RunGame(DIContainer projectContainter)
    {
        yield return Timing.WaitForOneFrame;

        BootSceneEntryPoint bootSceneEntryPoint = GameObject.FindFirstObjectByType<BootSceneEntryPoint>();
        bootSceneEntryPoint.Run(projectContainter, null);
    }
}
