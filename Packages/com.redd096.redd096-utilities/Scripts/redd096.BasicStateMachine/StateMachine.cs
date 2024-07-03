using redd096.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace redd096.BasicStateMachine
{
    public abstract class StateMachine : MonoBehaviour
    {
        [Header("DEBUG")]
        [ReadOnly] private State currentState = default;
        [ReadOnly][SerializeField] List<string> blackboardDebug = new List<string>();

        //blackboard to save vars to use in differents states
        private Dictionary<string, object> blackboard = new Dictionary<string, object>();

        //events
        public System.Action<State> onSetState { get; set; }
        public System.Action<string> onSetBlackboardValue { get; set; }

        protected virtual void Update()
        {
            if (currentState != null)
            {
                //update state
                currentState.Update();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (currentState != null)
            {
                //fixed update state
                currentState.FixedUpdate();
            }
        }

        #region public API

        /// <summary>
        /// Call it to change state
        /// </summary>
        public virtual void SetState(State stateToSet)
        {
            //exit from previous state
            if (currentState != null)
            {
                currentState.Exit();
            }

            //set new state
            currentState = stateToSet;

            //enter in new state
            if (currentState != null)
            {
                currentState.Initialize(this);
                currentState.Enter();
            }

            //call event
            onSetState?.Invoke(currentState);
        }

        /// <summary>
        /// Return CurrentState casted to inherited class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCurrentState<T>() where T : State
        {
            return currentState as T;
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