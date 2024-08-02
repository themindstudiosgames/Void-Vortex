using System;
using System.Collections.Generic;

namespace Utils.StateMachine
{
    public class StateMachine
    {
        private readonly Dictionary<Type, IExitableState> _states = new Dictionary<Type, IExitableState>();
        private IExitableState _activeState;

        protected void AddNewState(IExitableState state)
        {
            _states.Add(state.GetType(), state);
        }

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }

        public void Enter<TState, TPayload1, TPayload2>(TPayload1 payload1, TPayload2 payload2)
            where TState : class, IPayloadedState<TPayload1, TPayload2>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload1, payload2);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();

            TState state = GetState<TState>();
            _activeState = state;

            return state;
        }

        private TState GetState<TState>() where TState : class, IExitableState =>
            _states[typeof(TState)] as TState;
    }
}