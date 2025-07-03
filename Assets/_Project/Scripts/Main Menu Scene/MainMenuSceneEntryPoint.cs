using DI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneEntryPoint : SceneEntryPoint
{
    public override void Run(StateEnterData stateEnterData, out DIContainer sceneContainer)
    {
        sceneContainer = new DIContainer(ProjectContainer);

        InitializeDIContainer(sceneContainer, stateEnterData);

        FindObjectOfType<FSMUpdater>().Initialize(sceneContainer.Resolve<FSM>());

        var transitionCanvas = sceneContainer.Resolve<DownUpTransitionCanvas>();

        sceneContainer.Resolve<FSM>().OnStateChanged += transitionCanvas.Show;
    }

    private void InitializeDIContainer(DIContainer sceneContainer, StateEnterData enterData)
    {
        FsmRegistrations.Perform(sceneContainer, ProjectContainer, enterData);

        InputRegistrations.Perform(sceneContainer);

        sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<DownUpTransitionCanvas>(FindObjectsInactive.Include));
    }

    private static class InputRegistrations
    {
        public static void Perform(DIContainer sceneContainer)
        {
            sceneContainer.RegisterInstance(new CommonUIInputSourceTypeDependenceDispatcher(
                GameObject.FindObjectsByType<CommonUIInputSourceTypeDependence>(FindObjectsInactive.Include,
                FindObjectsSortMode.None),
                sceneContainer.Resolve<InputDispatcher>()));
        }
    }

    private static class FsmRegistrations
    {
        public static void Perform(DIContainer sceneContainer, DIContainer projectContainer, StateEnterData enterData)
        {
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<TitleScreenView>(FindObjectsInactive.Include));
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<LoadMenuView>(FindObjectsInactive.Include));
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<GameSettingsView>(FindObjectsInactive.Include));
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<MainMenuView>(FindObjectsInactive.Include));
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<CreditsView>(FindObjectsInactive.Include));
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<LoadGameplayView>(FindObjectsInactive.Include));

            sceneContainer.RegisterFactory((c) => new LoadMenuModel(c.Resolve<ISaveLoadSystem>())).AsSingle();
            sceneContainer.RegisterFactory((c) => new LoadMenuViewModel(c.Resolve<LoadMenuModel>())).AsSingle(); 
            
            sceneContainer.RegisterFactory((c) => new MainMenuViewModel(c.Resolve<ISaveLoadSystem>().SavedStateCount)).AsSingle();

            sceneContainer.RegisterFactory((c) => new TitleState(c.Resolve<TitleScreenView>(), c.Resolve<InputDispatcher>())).AsSingle();
            sceneContainer.RegisterFactory((c) => new MainMenuState(c.Resolve<MainMenuViewModel>(), c.Resolve<MainMenuView>(),
                c.Resolve<GameSettings>(), c.Resolve<InputDispatcher>())).AsSingle();
            sceneContainer.RegisterFactory((c) => new LoadMenuState(c.Resolve<LoadMenuModel>(),
                c.Resolve<LoadMenuViewModel>(), c.Resolve<LoadMenuView>(), c.Resolve<InputDispatcher>())).AsSingle();
            sceneContainer.RegisterFactory((c) => new SettingsState(c.Resolve<GameSettings>(), c.Resolve<GameSettingsView>(), c.Resolve<InputDispatcher>(),
                c.Resolve<ISaveLoadSystem>())).AsSingle();
            sceneContainer.RegisterFactory((c) => new CreditsState(c.Resolve<CreditsView>(), c.Resolve<InputDispatcher>())).AsSingle();
            sceneContainer.RegisterFactory((c) => new LoadGameplayState(c.Resolve<LoadGameplayView>(), projectContainer)).AsSingle();
            
            sceneContainer.RegisterInstance(GenerateFSM(sceneContainer, enterData));
        }

        private static FSM GenerateFSM(DIContainer sceneContainer, StateEnterData enterData)
        {
            TitleState titleState = sceneContainer.Resolve<TitleState>();
            MainMenuState mainMenuState = sceneContainer.Resolve<MainMenuState>();
            LoadMenuState loadMenuState = sceneContainer.Resolve<LoadMenuState>();
            SettingsState settingsState = sceneContainer.Resolve<SettingsState>();
            CreditsState creditsState = sceneContainer.Resolve<CreditsState>();
            LoadGameplayState loadGameplayState = sceneContainer.Resolve<LoadGameplayState>();

            IState[] states = new IState[]
            {
                titleState,
                mainMenuState,
                loadMenuState,
                settingsState,
                creditsState,
                loadGameplayState
            };

            Dictionary<Type, IStateTransition[]> transitions = new(states.Length);

            transitions[typeof(TitleState)] = new IStateTransition[]
            {
                new TitleToMainMenuTransition(titleState, mainMenuState)
            };

            transitions[typeof(MainMenuState)] = new IStateTransition[]
            {
                new MainMenuToSettingsTransition(mainMenuState, settingsState),
                new MainMenuToLoadMenuTransition(mainMenuState, loadMenuState),
                new MainMenuToGameplayTransition(mainMenuState, loadGameplayState)
            };

            transitions[typeof(LoadMenuState)] = new IStateTransition[]
            {
                new LoadMenuToMainMenuTransition(loadMenuState, mainMenuState),
                new LoadMenuToGameplayTransition(loadMenuState, loadGameplayState)
            };

            transitions[typeof(SettingsState)] = new IStateTransition[]
            {
                new SettingsToMainMenuTransition(settingsState, mainMenuState),
                new SettingsToCreditsTransition(settingsState, creditsState)
            };

            transitions[typeof(CreditsState)] = new IStateTransition[]
            {
                new CreditsToMainMenuTransition(creditsState, mainMenuState)
            };

            IState initState = titleState;

            if (enterData is MainMenuEnterData)
            {
                initState = mainMenuState;
            }

            FSM fsm = new FSM(states, transitions, initState, enterData);

            return fsm;
        }
    }
}
