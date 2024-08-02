using Data.DataProxy;
using Infrastructure.ClientAPI;
using Infrastructure.GameStateMachine.GameStates;
using SceneManagement;
using Screens;
using Utils.StateMachine;
using Zenject;

namespace Infrastructure.GameStateMachine
{
    public class GameStateMachine : StateMachine, IInitializable
    {
        public GameStateMachine(ScreenNavigationSystem screenNavigationSystem, SceneLoader sceneLoader,
            UserDataProxy userDataProxy, IApiInterface apiInterface)
        {
            AddNewState(new BootstrapState(sceneLoader, this, screenNavigationSystem, userDataProxy, apiInterface));
            AddNewState(new LoadLevelState(sceneLoader, this, screenNavigationSystem));
            AddNewState(new GameLoopState());
        }

        public void Initialize()
        {
            Enter<BootstrapState>();
        }
    }
}