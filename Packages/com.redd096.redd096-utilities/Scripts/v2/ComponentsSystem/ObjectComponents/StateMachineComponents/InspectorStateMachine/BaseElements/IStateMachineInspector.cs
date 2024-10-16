using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    public interface IStateMachineInspector
    {
        Transform transform { get; }
        InspectorState[] States { get; set; }
        InspectorState CurrentState { get; set; }

        System.Action<InspectorState> onSetState { get; set; }

        /// <summary>
        /// Exit from previous state and enter in new one by state index in States array
        /// </summary>
        /// <param name="nextState"></param>
        void SetState(int nextState);
        /// <summary>
        /// Exit from previous state and enter in new one by state name
        /// </summary>
        /// <param name="nextState"></param>
        void SetState(string nextState);
        /// <summary>
        /// Set state as Null
        /// </summary>
        void SetNullState();
        /// <summary>
        /// Set to first state
        /// </summary>
        void ResetToFirstState();
        /// <summary>
        /// Return CurrentState casted to inherited class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetCurrentState<T>() where T : InspectorState;
        /// <summary>
        /// Return CurrentState name
        /// </summary>
        /// <returns></returns>
        string GetCurrentStateName();
    }
}