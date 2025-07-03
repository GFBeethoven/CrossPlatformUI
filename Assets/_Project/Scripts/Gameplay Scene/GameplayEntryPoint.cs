using DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameplayEntryPoint : SceneEntryPoint
{
    public override void Run(StateEnterData stateEnterData, out DIContainer sceneContainer)
    {
        sceneContainer = new DIContainer(ProjectContainer);

        FsmRegistrations.Perform(sceneContainer, ProjectContainer, stateEnterData);

        sceneContainer.RegisterInstance(new CommonUIInputSourceTypeDependenceDispatcher(GameObject.FindObjectsByType<CommonUIInputSourceTypeDependence>(FindObjectsInactive.Include, FindObjectsSortMode.None),
            sceneContainer.Resolve<InputDispatcher>()));

        GameObject.FindFirstObjectByType<FSMUpdater>().Initialize(sceneContainer.Resolve<FSM>());
    }

    private static class FsmRegistrations
    {
        public static void Perform(DIContainer sceneContainer, DIContainer projectContainer, StateEnterData enterData)
        {
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<GameplayView>(FindObjectsInactive.Include));
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<PauseView>(FindObjectsInactive.Include));
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<LostControllerView>(FindObjectsInactive.Include));
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<LoadMainMenuView>(FindObjectsInactive.Include));
            sceneContainer.RegisterInstance(GameObject.FindFirstObjectByType<GameplaySplitScreen>(FindObjectsInactive.Include));

            sceneContainer.RegisterFactory((c) => new GameplayModel()).AsSingle();
            sceneContainer.RegisterFactory((c) => new GameplayViewModel(c.Resolve<GameplayModel>())).AsSingle();

            sceneContainer.RegisterFactory((c) => new GameplayState(c.Resolve<GameplayView>(),
                c.Resolve<GameplayModel>(), c.Resolve<GameplayViewModel>(), 
                c.Resolve<GameplaySplitScreen>(), c.Resolve<InputDispatcher>(),
                c.Resolve<GameSettings>())).AsSingle();
            sceneContainer.RegisterFactory((c) => new PauseState(c.Resolve<PauseView>(), c.Resolve<ISaveLoadSystem>())).AsSingle();
            sceneContainer.RegisterFactory((c) => new LostControllerState(c.Resolve<LostControllerView>())).AsSingle();
            sceneContainer.RegisterFactory((c) => new LoadMainMenuState(c.Resolve<LoadMainMenuView>(), projectContainer, c.Resolve<GameSettings>(), c.Resolve<ISaveLoadSystem>())).AsSingle();
            
            sceneContainer.RegisterInstance(GenerateFSM(sceneContainer, enterData));
        }

        private static FSM GenerateFSM(DIContainer sceneContainer, StateEnterData enterData)
        {
            GameplayState gameplayState = sceneContainer.Resolve<GameplayState>();
            LostControllerState lostControllerState = sceneContainer.Resolve<LostControllerState>();
            PauseState pauseState = sceneContainer.Resolve<PauseState>();
            LoadMainMenuState loadMainMenu = sceneContainer.Resolve<LoadMainMenuState>();

            IState[] states = new IState[]
            {
                gameplayState,
                lostControllerState,
                pauseState,
                loadMainMenu
            };

            Dictionary<Type, IStateTransition[]> transitions = new(states.Length);

            transitions[typeof(GameplayState)] = new IStateTransition[]
            {
                new GameplayToLostControllerTransition(gameplayState, lostControllerState),
                new GameplayToPauseTransition(gameplayState, pauseState)
            };

            transitions[typeof(LostControllerState)] = new IStateTransition[]
            {
                new LostControllerToGameplayTransition(lostControllerState, gameplayState),
                new LostControllerToMainMenuTransition(lostControllerState, loadMainMenu)
            };

            transitions[typeof(PauseState)] = new IStateTransition[]
            {
                new PauseToGameplayTransition(pauseState, gameplayState),
                new PauseToMainManuTransition(pauseState, loadMainMenu)
            };

            FSM fsm = new FSM(states, transitions, gameplayState, enterData);

            return fsm;
        }
    }
}
