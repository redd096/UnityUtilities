using UnityEngine;

namespace redd096.StateMachine
{
    /// <summary>
    /// Basic StateMachine where T is the casted StateMachine to use in various States
    /// </summary>
    /// <typeparam name="T">StateMachine cast passed to the States</typeparam>
    public class StateMachine<T> : MonoBehaviour where T : StateMachine<T>
    {
        protected IState<T> currentState;
        protected T stateMachine;

        public IState<T> CurrentState => currentState;
        public System.Action<IState<T>> OnSetState;

        protected virtual void Awake()
        {
            stateMachine = (T)this;
        }

        protected virtual void Update()
        {
            currentState?.UpdateState();
        }

        /// <summary>
        /// Exit from previous state and enter in new one
        /// </summary>
        /// <param name="state"></param>
        public virtual void SetState(IState<T> state)
        {
            currentState?.Exit();

            //set new state and set its statemachine
            currentState = state;
            currentState.StateMachine = stateMachine;

            currentState?.Enter();

            //call event
            OnSetState?.Invoke(currentState);
        }
    }
}