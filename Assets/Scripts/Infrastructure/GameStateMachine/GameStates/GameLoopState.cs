using UnityEngine;
using Utils.StateMachine;

namespace Infrastructure.GameStateMachine.GameStates
{
    public class GameLoopState : IState
    {
        public void Enter()
        {
            DynamicGI.UpdateEnvironment();
        }

        public void Exit()
        {
            
        }
    }
}