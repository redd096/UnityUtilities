using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Create a state machine and set every State in inspector.
    /// If you think at this as the Animator: every box is a State, every StateMachineBehaviour is an Action to add to the State, every arrow is a Transition from the State to another State
    /// </summary>
    [System.Serializable]
    public class InspectorStateMachineComponent : IStateMachineInspector, IBlackboard, IComponentRD
    {
        [SerializeField] bool setFirstStateOnStart = true;
        [SerializeField] InspectorState[] states = default;

        //statemachine vars
        public IGameObjectRD Owner { get; set; }
        public Transform transform => Owner?.transform;
        public InspectorState[] States { get => states; set => states = value; }
        public InspectorState CurrentState { get; set; }

        //blackboard to save vars to use in differents tasks (key: variable name, value: variable value)
        public Dictionary<string, object> blackboard { get; set; } = new Dictionary<string, object>();
        public List<string> blackboardDebug { get; set; } = new List<string>();

        //events
        public System.Action<InspectorState> onSetState { get; set; }
        public System.Action<string> onSetBlackboardValue { get; set; }

        enum EUpdateTask { Update, FixedUpdate, LateUpdate }

        public virtual void StartRD()
        {
            //start with first state
            //set in Start because in Awake normally we are still doing things like PlayerController.Possess(PlayerPawn)
            if (setFirstStateOnStart)
                SetState(0);
        }

        public virtual void UpdateRD()
        {
            if (CurrentState != null)
            {
                //do every action
                DoActions(EUpdateTask.Update);

                //check every transition
                CheckTransitions();
            }
        }

        public virtual void FixedUpdateRD()
        {
            if (CurrentState != null)
            {
                //do every action fixed update
                DoActions(EUpdateTask.FixedUpdate);
            }
        }

        public virtual void LateUpdateRD()
        {
            if (CurrentState != null)
            {
                //do every action fixed update
                DoActions(EUpdateTask.LateUpdate);
            }
        }

        #region public API

        /// <summary>
        /// Exit from previous state and enter in new one by state index in States array
        /// </summary>
        /// <param name="nextState"></param>
        public virtual void SetState(int nextState)
        {
            //exit from previous state
            if (CurrentState != null)
            {
                ExitState();
            }

            //set new state
            CurrentState = nextState >= 0 && states != null && states.Length > nextState ? states[nextState] : null;

            //enter in new state
            if (CurrentState != null)
            {
                EnterState();
            }

            //call event
            onSetState?.Invoke(CurrentState);
        }

        /// <summary>
        /// Exit from previous state and enter in new one by state name
        /// </summary>
        /// <param name="nextState"></param>
        public virtual void SetState(string nextState)
        {
            if (states != null)
            {
                //find state name and set it
                for (int i = 0; i < states.Length; i++)
                {
                    if (nextState.Equals(states[i].StateName))//, System.StringComparison.CurrentCultureIgnoreCase)
                    {
                        SetState(i);
                        return;
                    }
                }
            }

            //else set null as state
            SetState(-1);
        }

        /// <summary>
        /// Set state as Null
        /// </summary>
        public void SetNullState()
        {
            SetState(-1);
        }

        /// <summary>
        /// Set to first state
        /// </summary>
        public void ResetToFirstState()
        {
            SetState(0);
        }

        /// <summary>
        /// Return CurrentState casted to inherited class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCurrentState<T>() where T : InspectorState
        {
            return CurrentState as T;
        }

        /// <summary>
        /// Return CurrentState name
        /// </summary>
        /// <returns></returns>
        public string GetCurrentStateName()
        {
            return CurrentState != null ? CurrentState.StateName : null;
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

        #region private API

        void DoActions(EUpdateTask updateTask)
        {
            if (CurrentState.Actions == null)
                return;

            //do every action
            for (int i = 0; i < CurrentState.Actions.Length; i++)
            {
                if (CurrentState.Actions[i] != null)
                {
                    if (updateTask == EUpdateTask.Update)
                        CurrentState.Actions[i].UpdateTask();
                    else if (updateTask == EUpdateTask.FixedUpdate)
                        CurrentState.Actions[i].FixedUpdateTask();
                    else if (updateTask == EUpdateTask.LateUpdate)
                        CurrentState.Actions[i].LateUpdateTask();
                }
            }
        }

        void CheckTransitions()
        {
            if (CurrentState.Transitions == null)
                return;

            //check every transition
            for (int i = 0; i < CurrentState.Transitions.Length; i++)
            {
                if (CurrentState.Transitions[i] != null)
                {
                    //if conditions return true, set new state
                    if ((CurrentState.Transitions[i].TransitionCheck == ETransitionCheck.AllTrueRequired && CheckConditionsEVERY(CurrentState.Transitions[i]))
                        || (CurrentState.Transitions[i].TransitionCheck == ETransitionCheck.AnyTrueSuffice && CheckConditionsANY(CurrentState.Transitions[i])))
                    {
                        SetState(CurrentState.Transitions[i].DestinationState);

                        //break, because now is in another state
                        break;
                    }
                }
            }
        }

        bool CheckConditionsEVERY(Transition transition)
        {
            if (transition.Conditions == null)
                return false;

            //return true if EVERY condition is true
            for (int i = 0; i < transition.Conditions.Count; i++)
            {
                if (transition.Conditions[i] != null)
                {
                    //if anyone is false, return false
                    if (transition.Conditions[i].CheckTask() == false)
                        return false;
                }
            }

            return true;
        }

        bool CheckConditionsANY(Transition transition)
        {
            if (transition.Conditions == null)
                return false;

            //return true if ANY condition is true
            for (int i = 0; i < transition.Conditions.Count; i++)
            {
                if (transition.Conditions[i] != null)
                {
                    //if anyone is true, return true
                    if (transition.Conditions[i].CheckTask())
                        return true;
                }
            }

            return false;
        }

        void ExitState()
        {
            //exit from actions
            if (CurrentState.Actions != null)
            {
                foreach (ActionTask action in CurrentState.Actions)
                    action.ExitTask();
            }

            //exit from previous conditions
            if (CurrentState.Transitions != null)
            {
                foreach (Transition transition in CurrentState.Transitions)
                    if (transition != null && transition.Conditions != null)
                        foreach (ConditionTask condition in transition.Conditions)
                            condition.ExitTask();
            }
        }

        void EnterState()
        {
            //enter in new actions
            if (CurrentState.Actions != null)
            {
                foreach (ActionTask action in CurrentState.Actions)
                {
                    action.InitializeTask(this, this);
                    action.EnterTask();
                }
            }

            //enter in new conditions
            if (CurrentState.Transitions != null)
            {
                foreach (Transition transition in CurrentState.Transitions)
                {
                    if (transition != null && transition.Conditions != null)
                    {
                        foreach (ConditionTask condition in transition.Conditions)
                        {
                            condition.InitializeTask(this, this);
                            condition.EnterTask();
                        }
                    }
                }
            }

        }

        #endregion
    }
}