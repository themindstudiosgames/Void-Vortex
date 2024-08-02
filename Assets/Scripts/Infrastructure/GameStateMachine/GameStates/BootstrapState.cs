using System;
using System.Reflection;
using Data.DataProxy;
using Data.Enums;
using Infrastructure.ClientAPI;
using SAS_SDK.Utils;
using SceneManagement;
using Screens;
using Sounds;
using UnityEditor;
using UnityEngine;
using Utils.StateMachine;

namespace Infrastructure.GameStateMachine.GameStates
{
    public class BootstrapState : IState
    {
        private readonly SceneLoader _sceneLoader;
        private readonly GameStateMachine _gameStateMachine;
        private readonly ScreenNavigationSystem _screenNavigationSystem;
        private readonly UserDataProxy _userDataProxy;
        private readonly IApiInterface _apiInterface;

        public BootstrapState(SceneLoader sceneLoader, GameStateMachine gameStateMachine,
            ScreenNavigationSystem screenNavigationSystem, UserDataProxy userDataProxy,
            IApiInterface apiInterface)
        {
            _sceneLoader = sceneLoader;
            _gameStateMachine = gameStateMachine;
            _screenNavigationSystem = screenNavigationSystem;
            _userDataProxy = userDataProxy;
            _apiInterface = apiInterface;
        }

        public void Enter()
        {
            _sceneLoader.Load(SceneNames.BootScene, OnBootSceneLoaded);
        }

        private void OnBootSceneLoaded()
        {
#if UNITY_EDITOR
            ClearEditorLogs();
#endif
            Debug.Log("Loaded BootScene");
            Initialize();
        }

#if UNITY_EDITOR
        private static void ClearEditorLogs()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Editor));
            Type type = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
#endif

        private void Initialize()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _screenNavigationSystem.ExecuteNavigationCommand(new NavigationCommand().ShowNextScreen(ScreenName.LoadingScreen).WithExtraData(false));
            
            DeviceIDGetter.GetID(deviceId =>
            {
                _apiInterface.LoginWithDeviceID(deviceId,
                    loginResult =>
                    {
                        if (loginResult.NewlyCreated)
                        {
                            _userDataProxy.ChangeLoginState(LoginStateType.NicknameNeeded);
                            _userDataProxy.UserPlayFabID = loginResult.PlayFabId;
                            _gameStateMachine.Enter<LoadLevelState, SceneNames>(SceneNames.MainScene);
                        }
                        else
                        {
                            _apiInterface.GetPlayerProfile(
                                result =>
                                {
                                    _userDataProxy.ChangeNickname(result.PlayerProfile.DisplayName);
                                    _userDataProxy.UserPlayFabID = result.PlayerProfile.PlayerId;

                                    _userDataProxy.ChangeLoginState(
                                        string.IsNullOrEmpty(result.PlayerProfile.DisplayName)
                                            ? LoginStateType.NicknameNeeded
                                            : LoginStateType.Logged);
                                    _gameStateMachine.Enter<LoadLevelState, SceneNames>(SceneNames.MainScene);
                                });
                        }
                    });
            });
            
            
        }

        public void Exit()
        {
        }
    }
}