using System.Collections.Generic;
using UnityEngine;
//using NaughtyAttributes;

namespace redd096
{
    #region classes and enums

    public enum ETransitionCheck { AllTrueRequired, AnyTrueSuffice }

    [System.Serializable]
    public class Transition
    {
        [DropdownState("Destination State")] public int StateDestination;
        public ETransitionCheck TransitionCheck;
        [DropdownTask(typeof(ConditionTask))] public List<ConditionTask> Conditions;
    }

    [System.Serializable]
    public class State
    {
        public string StateName;
        [DropdownTask(typeof(ActionTask))] public ActionTask[] Actions;
        public Transition[] Transitions;
    }

    #endregion

    [AddComponentMenu("redd096/StateMachineRedd096/State Machine redd096")]
    public class StateMachineRedd096 : MonoBehaviour
    {
        //[ReorderableList] 
        public State[] States = default;

        //[BoxGroup("DEBUG")] [ReadOnly] 
        public State CurrentState = default;
        //[BoxGroup("DEBUG")] [ReadOnly] 
        [SerializeField] List<string> blackboardDebug = new List<string>();

        //blackboard to save vars to use in differents tasks
        Dictionary<string, object> blackboard = new Dictionary<string, object>();

        void Start()
        {
            //start with first state
            SetState(0);
        }

        void Update()
        {
            if (CurrentState != null)
            {
                //do every action
                DoActions();

                //check every transition
                CheckTransitions();
            }
        }

        /// <summary>
        /// Exit from previous state and enter in new one
        /// </summary>
        /// <param name="nextState"></param>
        public void SetState(int nextState)
        {
            //exit from previous state
            if (CurrentState != null)
            {
                ExitState();
            }

            //set new state
            CurrentState = nextState >= 0 && States != null && States.Length > nextState ? States[nextState] : null;

            //enter in new state
            if(CurrentState != null)
            {
                EnterState();
            }
        }

        #region private API

        void DoActions()
        {
            //do every action
            for (int i = 0; i < CurrentState.Actions.Length; i++)
            {
                if (CurrentState.Actions[i] != null)
                    CurrentState.Actions[i].OnUpdateTask();
            }
        }

        void CheckTransitions()
        {
            //check every transition
            for (int i = 0; i < CurrentState.Transitions.Length; i++)
            {
                if (CurrentState.Transitions[i] != null)
                {
                    //if conditions return true, set new state
                    if ( (CurrentState.Transitions[i].TransitionCheck == ETransitionCheck.AllTrueRequired && CheckConditionsEVERY(CurrentState.Transitions[i]))
                        || (CurrentState.Transitions[i].TransitionCheck == ETransitionCheck.AnyTrueSuffice && CheckConditionsANY(CurrentState.Transitions[i])))
                    {
                        SetState(CurrentState.Transitions[i].StateDestination);

                        //break, because now is in another state
                        break;
                    }
                }
            }
        }

        bool CheckConditionsEVERY(Transition transition)
        {
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
            //return true if ANY condition is true
            for(int i = 0; i< transition.Conditions.Count; i++)
            {
                if(transition.Conditions[i] != null)
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
            foreach (ActionTask action in CurrentState.Actions)
                if (action)
                    action.OnExitTask();

            //exit from previous conditions
            foreach (Transition transition in CurrentState.Transitions)
                if (transition != null)
                    foreach (ConditionTask condition in transition.Conditions)
                        if (condition)
                            condition.OnExitTask();
        }

        void EnterState()
        {
            //enter in new actions
            foreach (ActionTask action in CurrentState.Actions)
            {
                if (action)
                {
                    action.InitializeTask(this);
                    action.OnEnterTask();
                }
            }

            //enter in new conditions
            foreach (Transition transition in CurrentState.Transitions)
            {
                if (transition != null)
                {
                    foreach (ConditionTask condition in transition.Conditions)
                    {
                        if (condition)
                        {
                            condition.InitializeTask(this);
                            condition.OnEnterTask();
                        }
                    }
                }
            }
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
            if(Application.isEditor)
            {
                //add value in blackboard
                if (blackboardDebug.Contains(key) == false)
                {
                    blackboardDebug.Add(key);
                }
            }
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
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetBlackboardElement(string key)
        {
            //return element from blackboard
            if (blackboard.ContainsKey(key))
                return blackboard[key];

            //if there is no key, return null
            return null;
        }

        #endregion
    }
}