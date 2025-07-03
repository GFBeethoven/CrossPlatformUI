using DI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Audio;
using Lean.Localization;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class BootSceneEntryPoint : SceneEntryPoint
{
    [SerializeField] private GameObject _sceneSharedObjects;
    [SerializeField] private AudioMixer _mainMixer;
    [SerializeField] private ControlIcons _controlIcons;
    [SerializeField] private InputActionAsset _inputActionAsset;
    [SerializeField] private InputActionAsset _defaultActionAsset;

    public override void Run(StateEnterData stateEnterData, out DIContainer sceneContainer)
    {
        ILoadingOperation[] operations = new ILoadingOperation[]
        {
            new DetermineLanguageOperation(ProjectContainer),
            new LoadGameSettingsOperation(ProjectContainer),
            new WaitAPIInitializeOperation(),
            new LoadGameStateOperation(ProjectContainer)
        };

        InitializeProjectDIContainer(ProjectContainer);

        DIContainer bootSceneContainer = new DIContainer(ProjectContainer);

        InitializeBootDIContainer(bootSceneContainer);

        LoadingScreen loadingScreen = GameObject.FindFirstObjectByType<LoadingScreen>();

        loadingScreen.Initialize(bootSceneContainer.Resolve<Popups>());
        loadingScreen.Show();
        loadingScreen.PerformLoadingOperations(operations, LoadingSuccess, LoadingFailure);

        sceneContainer = bootSceneContainer;
    }

    private void InitializeProjectDIContainer(DIContainer projectContainer)
    {
        DontDestroyOnLoad(Instantiate(_sceneSharedObjects, null));

        projectContainer.RegisterInstance(GameObject.FindFirstObjectByType<AudioPlayer>(FindObjectsInactive.Include));
        projectContainer.RegisterInstance(new InputDispatcher(_inputActionAsset.FindActionMap("Gameplay"), 
            _inputActionAsset.FindActionMap("UI"), _defaultActionAsset.FindActionMap("UI")));
        projectContainer.RegisterInstance(GameObject.FindFirstObjectByType<InputDispatcherUpdater>());
        projectContainer.Resolve<InputDispatcherUpdater>().Setup(projectContainer.Resolve<InputDispatcher>());
        projectContainer.RegisterInstance(_mainMixer);
        projectContainer.RegisterInstance<ISaveLoadSystem>(new JsonSaveLoadSystem());
        projectContainer.RegisterInstance(GameObject.FindFirstObjectByType<GameViewport>(FindObjectsInactive.Include));
        projectContainer.RegisterInstance(GameObject.FindFirstObjectByType<LeanLocalization>(FindObjectsInactive.Include));
        projectContainer.RegisterInstance(new InputSourceIcons(_controlIcons));
        projectContainer.RegisterInstance(GameObject.FindFirstObjectByType<UIFocusGamepadRemainer>(FindObjectsInactive.Include));

        projectContainer.Resolve<GameViewport>().Initialize();
        projectContainer.Resolve<AudioPlayer>().Initialize();
        projectContainer.Resolve<UIFocusGamepadRemainer>().Initialize(projectContainer.Resolve<InputDispatcher>());
    }

    private void InitializeBootDIContainer(DIContainer container)
    {
        Popups popups = GameObject.FindFirstObjectByType<Popups>();
        popups.Initialize();

        container.RegisterInstance(popups);
    }

    private void LoadingSuccess()
    {
        Timing.RunCoroutine(_CallMainSceneEntryPoint());

        SceneManager.LoadScene(Scenes.MainMenu);
    }

    private void LoadingFailure(ILoadingOperation failedOperation)
    {
        Debug.LogError($"Failed to load: {failedOperation.GetType().FullName}");
    }

    private IEnumerator<float> _CallMainSceneEntryPoint()
    {
        while (SceneManager.GetActiveScene().name != Scenes.MainMenu)
        {
            yield return Timing.WaitForOneFrame;
        }

        GameObject.FindFirstObjectByType<MainMenuSceneEntryPoint>().Run(ProjectContainer, StateEnterData.Empty);
    }
}
