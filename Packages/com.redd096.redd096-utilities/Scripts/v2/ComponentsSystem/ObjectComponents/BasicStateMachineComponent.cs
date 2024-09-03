using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Create a state machine from this class
    /// </summary>
    public class BasicStateMachineComponent : IStateMachineBasic, IComponentRD
    {
        //statemachine vars
        public IGameObjectRD Owner { get; set; }
        public Transform transform => Owner?.transform;
        public State CurrentState { get; set; }

        protected string currentStateDebug;


        //blackboard to save vars to use in differents states (key: variable name, value: variable value)
        public Dictionary<string, object> blackboard { get; set; } = new Dictionary<string, object>();
        public List<string> blackboardDebug { get; set; } = new List<string>();

        //events
        public System.Action<State> onSetState { get; set; }
        public System.Action<string> onSetBlackboardValue { get; set; }

        public virtual void UpdateRD()
        {
            if (CurrentState != null)
            {
                //update state
                CurrentState.Update();
            }
        }

        public virtual void FixedUpdateRD()
        {
            if (CurrentState != null)
            {
                //fixed update state
                CurrentState.FixedUpdate();
            }
        }

        public virtual void LateUpdateRD()
        {
            if (CurrentState != null)
            {
                //late update state
                CurrentState.LateUpdate();
            }
        }

        #region public API

        /// <summary>
        /// Exit from previous state and enter in new one
        /// </summary>
        /// <param name="stateToSet"></param>
        public virtual void SetState(State stateToSet)
        {
            //exit from previous state
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            //set new state
            CurrentState = stateToSet;
            if (Application.isEditor) currentStateDebug = CurrentState != null ? CurrentState.ToString() : "";

            //enter in new state
            if (CurrentState != null)
            {
                CurrentState.Initialize(this);
                CurrentState.Enter();
            }

            //call event
            onSetState?.Invoke(CurrentState);
        }

        /// <summary>
        /// Set state as Null
        /// </summary>
        public void SetNullState()
        {
            SetState(null);
        }

        /// <summary>
        /// Return CurrentState casted to inherited class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCurrentState<T>() where T : State
        {
            return CurrentState as T;
        }

        #endregion

        #region blackboard public API

        /// <summary>
        /// Add or Set element inside blackboard
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBlackboardElement(string key, object value)
        {
            //add element if not inside blackboard
            if (blackboard.ContainsKey(key) == false)
            {
                blackboard.Add(key, value);
            }
            //else set it
            else
            {
                blackboard[key] = value;
            }

            //in editor, update debug blackboard
            if (Application.isEditor)
            {
                //add value in blackboard
                if (blackboardDebug.Contains(key) == false)
                {
                    blackboardDebug.Add(key);
                }
            }

            //call event
            onSetBlackboardValue?.Invoke(key);
        }

        /// <summary>
        /// Remove element from blackboard
        /// </summary>
        /// <param name="key"></param>
        public void RemoveBlackboardElement(string key)
        {
            //remove element from blackboard
            if (blackboard.ContainsKey(key))
                blackboard.Remove(key);

            //in editor, update debug blackboard
            if (Application.isEditor)
            {
                //remove value from blackboard
                if (blackboardDebug.Contains(key))
                {
                    blackboardDebug.Remove(key);
                }
            }
        }

        /// <summary>
        /// Get element from blackboard
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetBlackboardElement<T>(string key)
        {
            //return element from blackboard
            if (blackboard.ContainsKey(key))
            {
                return (T)blackboard[key];
            }

            //if there is no key, return null
            return default;
        }

        #endregion
    }
}