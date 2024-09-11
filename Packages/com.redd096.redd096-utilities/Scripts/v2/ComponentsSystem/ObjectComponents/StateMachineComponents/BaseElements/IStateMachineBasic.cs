using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    public interface IStateMachineBasic
    {
        Transform transform { get; }
        State CurrentState { get; set; }

        System.Action<State> onSetState { get; set; }

        /// <summary>
        /// Exit from previous state and enter in new one
        /// </summary>
        /// <param name="stateToSet"></param>
        void SetState(State stateToSet);
        /// <summary>
        /// Set state as Null
        /// </summary>
        void SetNullState();
        /// <summary>
        /// Return CurrentState casted to inherited class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetCurrentState<T>() where T : State;
    }
}