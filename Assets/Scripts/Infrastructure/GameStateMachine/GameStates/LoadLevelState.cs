using System;
using SceneManagement;
using Screens;
using UnityEngine;
using Utils.StateMachine;
using Observable = UniRx.Observable;
using UniRx;

namespace Infrastructure.GameStateMachine.GameStates
{
    public class LoadLevelState : IPayloadedState<SceneNames>
    {
        private const float SceneLoadDelay = 1;
        private readonly SceneLoader _sceneLoader;
        private readonly GameStateMachine _gameStateMachine;
        private readonly ScreenNavigationSystem _screenNavigationSystem;

        public LoadLevelState(SceneLoader sceneLoader, GameStateMachine gameStateMachine,
            ScreenNavigationSystem screenNavigationSystem)
        {
            _sceneLoader = sceneLoader;
            _gameStateMachine = gameStateMachine;
            _screenNavigationSystem = screenNavigationSystem;
        }

        public void Enter(SceneNames sceneName)
        {
            _sceneLoader.SetProgress(0.1f);
            _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().CloseAllScreens());
            _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand()
                .ShowNextScreen(ScreenName.LoadingScreen).WithExtraData(true));
            Observable.Timer(TimeSpan.FromSeconds(SceneLoadDelay)).Subscribe(delegate
            {
                _sceneLoader.SetProgress(0.5f);
                _sceneLoader.Load(sceneName, delegate { OnSceneLoaded(sceneName); });
            });
        }

        private void OnSceneLoaded(SceneNames sceneName)
        {
            Debug.Log("Loaded " + sceneName);
            _gameStateMachine.Enter<GameLoopState>();
        }

        public void Exit()
        {
            _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().CloseScreen(ScreenName.LoadingScreen));
        }
    }
}