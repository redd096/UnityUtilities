using System.Collections.Generic;
using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Create a state machine and set every State in inspector.
    /// If you think at this as the Animator: every box is a State, every StateMachineBehaviour is an Action to add to the State, every arrow is a Transition from the State to another State
    /// </summary>
    [System.Serializable]
    public class InspectorStateMachineComponent : ICharacterComponent, IBlackboard
    {
        [SerializeField] bool setFirstStateOnStart = true;
        public InspectorState[] States = default;

        public ICharacter Owner { get; set; }

        protected InspectorState currentState = default;

        //blackboard to save vars to use in differents tasks (key: variable name, value: variable value)
        public Dictionary<string, object> blackboard { get; set; } = new Dictionary<string, object>();
        public List<string> blackboardDebug { get; set; } = new List<string>();

        //events
        public System.Action<InspectorState> onSetState { get; set; }
        public System.Action<string> onSetBlackboardValue { get; set; }

        enum EUpdateTask { Update, FixedUpdate, LateUpdate }

        public virtual void Start()
        {
            //start with first state
            //set in Start because in Awake normally we are still doing things like PlayerController.Possess(PlayerPawn)
            if (setFirstStateOnStart)
                SetState(0);
        }

        public virtual void Update()
        {
            if (currentState != null)
            {
                //do every action
                DoActions(EUpdateTask.Update);

                //check every transition
                CheckTransitions();
            }
        }

        public virtual void FixedUpdate()
        {
            if (currentState != null)
            {
                //do every action fixed update
                DoActions(EUpdateTask.FixedUpdate);
            }
        }

        public virtual void LateUpdate()
        {
            if (currentState != null)
            {
                //do every action fixed update
                DoActions(EUpdateTask.LateUpdate);
            }
        }

        #region public API

        /// <summary>
        /// Exit from previous state and enter in new one
        /// </summary>
        /// <param name="nextState"></param>
        public virtual void SetState(int nextState)
        {
            //exit from previous state
            if (currentState != null)
            {
                ExitState();
            }

            //set new state
            currentState = nextState >= 0 && States != null && States.Length > nextState ? States[nextState] : null;

            //enter in new state
            if (currentState != null)
            {
                EnterState();
            }

            //call event
            onSetState?.Invoke(currentState);
        }

        /// <summary>
        /// Exit from previous state and enter in new one
        /// </summary>
        /// <param name="nextState"></param>
        public virtual void SetState(string nextState)
        {
            if (States != null)
            {
                //find state name and set it
                for (int i = 0; i < States.Length; i++)
                {
                    if (nextState.Equals(States[i].StateName))//, System.StringComparison.CurrentCultureIgnoreCase)
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
        /// Set state at Null
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

        #region private API

        void DoActions(EUpdateTask updateTask)
        {
            if (currentState.Actions == null)
                return;

            //do every action
            for (int i = 0; i < currentState.Actions.Length; i++)
            {
                if (currentState.Actions[i] != null)
                {
                    if (updateTask == EUpdateTask.Update)
                        currentState.Actions[i].OnUpdateTask();
                    else if (updateTask == EUpdateTask.FixedUpdate)
                        currentState.Actions[i].OnFixedUpdateTask();
                    else if (updateTask == EUpdateTask.LateUpdate)
                        currentState.Actions[i].OnLateUpdateTask();
                }
            }
        }

        void CheckTransitions()
        {
            if (currentState.Transitions == null)
                return;

            //check every transition
            for (int i = 0; i < currentState.Transitions.Length; i++)
            {
                if (currentState.Transitions[i] != null)
                {
                    //if conditions return true, set new state
                    if ((currentState.Transitions[i].TransitionCheck == ETransitionCheck.AllTrueRequired && CheckConditionsEVERY(currentState.Transitions[i]))
                        || (currentState.Transitions[i].TransitionCheck == ETransitionCheck.AnyTrueSuffice && CheckConditionsANY(currentState.Transitions[i])))
                    {
                        SetState(currentState.Transitions[i].DestinationState);

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
                    if (transition.Conditions[i].OnCheckTask() == false)
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
                    if (transition.Conditions[i].OnCheckTask())
                        return true;
                }
            }

            return false;
        }

        void ExitState()
        {
            //exit from actions
            if (currentState.Actions != null)
            {
                foreach (ActionTask action in currentState.Actions)
                    action.ExitTask();
            }

            //exit from previous conditions
            if (currentState.Transitions != null)
            {
                foreach (Transition transition in currentState.Transitions)
                    if (transition != null && transition.Conditions != null)
                        foreach (ConditionTask condition in transition.Conditions)
                            condition.ExitTask();
            }
        }

        void EnterState()
        {
            //enter in new actions
            if (currentState.Actions != null)
            {
                foreach (ActionTask action in currentState.Actions)
                {
                    action.InitializeTask(this);
                    action.EnterTask();
                }
            }

            //enter in new conditions
            if (currentState.Transitions != null)
            {
                foreach (Transition transition in currentState.Transitions)
                {
                    if (transition != null && transition.Conditions != null)
                    {
                        foreach (ConditionTask condition in transition.Conditions)
                        {
                            condition.InitializeTask(this);
                            condition.EnterTask();
                        }
                    }
                }
            }

        }

        #endregion
    }
}