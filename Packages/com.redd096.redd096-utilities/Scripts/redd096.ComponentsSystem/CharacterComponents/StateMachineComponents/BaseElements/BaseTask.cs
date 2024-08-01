using UnityEngine;

namespace redd096.ComponentsSystem
{
    /// <summary>
    /// This is the base class, from which inherit ActionTask and ConditionTask
    /// </summary>
    public abstract class BaseTask : MonoBehaviour
    {
        public string TaskName = "DefaultTask";

        private InspectorStateMachineComponent _stateMachine;
        public InspectorStateMachineComponent StateMachine { get => _stateMachine; set => _stateMachine = value; }
        private bool _isTaskActive;
        public bool IsTaskActive { get => _isTaskActive; set => _isTaskActive = value; }

        bool isInitialized;

        #region protected

        /// <summary>
        /// Use statemachine's transform if possible, else use this task's transform
        /// </summary>
        protected Transform transformTask => _stateMachine.Owner != null ? _stateMachine.Owner.transform : transform;

        /// <summary>
        /// Return StateMachine casted to inherited class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetStateMachine<T>() where T : InspectorStateMachineComponent
        {
            return _stateMachine as T;
        }

        /// <summary>
        /// Get component in stateMachine or its parent. If not found, show warning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="showWarningIfNotFound"></param>
        /// <returns></returns>
        protected T GetStateMachineComponent<T>(bool showWarningIfNotFound = true)
        {
            //get in parent
            T component = _stateMachine != null ? _stateMachine.Owner.transform.GetComponentInParent<T>() : default;

            //show warning if not found
            if (showWarningIfNotFound && component == null)
                Debug.LogWarning($"Miss {typeof(T).Name} on {_stateMachine}", _stateMachine.Owner.transform);

            return component;
        }

        /// <summary>
        /// Try get component in stateMachine or its parent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="foundComponent"></param>
        /// <returns></returns>
        protected bool TryGetStateMachineComponent<T>(out T foundComponent)
        {
            //get in parent
            foundComponent = _stateMachine != null ? _stateMachine.Owner.transform.GetComponentInParent<T>() : default;
            return foundComponent != null;
        }

        /// <summary>
        /// Get blackboard or normal value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        protected T GetValue<T>(VarOrBlackboard<T> v) => v != null ? v.GetValue(_stateMachine) : default;

        #endregion

        #region statemachine functions

        /// <summary>
        /// Called by StateMachine, to set owner and to be sure to call OnInitTask() only one time
        /// </summary>
        /// <param name="stateMachine"></param>
        public void InitializeTask(InspectorStateMachineComponent stateMachine)
        {
            if (isInitialized)
                return;

            isInitialized = true;

            //set state machine and init
            _stateMachine = stateMachine;
            OnInitTask();
        }

        /// <summary>
        /// Called by StateMachine, to set task active and call OnEnterTask()
        /// </summary>
        public void EnterTask()
        {
            _isTaskActive = true;
            OnEnterTask();
        }

        /// <summary>
        /// Called by StateMachine, to set task not active and call OnExitTask()
        /// </summary>
        public void ExitTask()
        {
            _isTaskActive = false;
            OnExitTask();
        }

        #endregion

        /// <summary>
        /// Called only one time, first time enter in this task
        /// </summary>
        protected virtual void OnInitTask() { }
        /// <summary>
        /// Called every time enter in this task
        /// </summary>
        protected virtual void OnEnterTask() { }
        /// <summary>
        /// Called every time exit from this task
        /// </summary>
        protected virtual void OnExitTask() { }
    }
}