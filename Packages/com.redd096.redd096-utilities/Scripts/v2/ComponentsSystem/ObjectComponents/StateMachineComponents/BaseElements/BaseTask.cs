using UnityEngine;

namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// This is the base class, from which inherit ActionTask and ConditionTask
    /// </summary>
    public abstract class BaseTask : MonoBehaviour
    {
        public string TaskName = "DefaultTask";

        private IStateMachine _stateMachine;
        public IStateMachine StateMachine { get => _stateMachine; set => _stateMachine = value; }
        private IComponentRD _stateMachineComponentRD;
        private bool _isTaskActive;
        public bool IsTaskActive { get => _isTaskActive; set => _isTaskActive = value; }

        bool isInitialized;

        #region protected

        /// <summary>
        /// Use statemachine's transform if possible, else use this task's transform
        /// </summary>
        protected Transform transformTask => _stateMachine != null ? _stateMachine.transform : transform;

        /// <summary>
        /// Return StateMachine casted to inherited class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetStateMachine<T>() where T : IStateMachine
        {
            return (T)_stateMachine;
        }

        /// <summary>
        /// Get unity component in stateMachine or its parents. If not found, show warning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="showWarningIfNotFound"></param>
        /// <returns></returns>
        protected T GetStateMachineUnityComponent<T>(bool showWarningIfNotFound = true)
        {
            //get in parent
            T component = _stateMachine != null ? _stateMachine.transform.GetComponentInParent<T>() : default;

            //show warning if not found
            if (showWarningIfNotFound && component == null)
                Debug.LogWarning($"Miss {typeof(T).Name} on {_stateMachine}", _stateMachine.transform.gameObject);

            return component;
        }

        /// <summary>
        /// Try get unity component in stateMachine or its parents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="foundComponent"></param>
        /// <returns></returns>
        protected bool TryGetStateMachineUnityComponent<T>(out T foundComponent)
        {
            //get in parent
            foundComponent = _stateMachine != null ? _stateMachine.transform.GetComponentInParent<T>() : default;
            return foundComponent != null;
        }

        /// <summary>
        /// Get IComponentRD in stateMachine's owner. If not found, show warning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="showWarningIfNotFound"></param>
        /// <returns></returns>
        protected T GetStateMachineComponentRD<T>(bool showWarningIfNotFound = true)
        {
            //get in owner
            T component = _stateMachineComponentRD != null ? _stateMachineComponentRD.Owner.GetComponentRD<T>() : default;

            //show warning if not found
            if (showWarningIfNotFound && component == null)
                Debug.LogWarning($"Miss {typeof(T).Name} on {_stateMachine}", _stateMachine.transform.gameObject);

            return component;
        }

        /// <summary>
        /// Try get IComponentRD in stateMachine's owner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="foundComponent"></param>
        /// <returns></returns>
        protected bool TryGetStateMachineComponentRD<T>(out T foundComponent)
        {
            //get in owner
            foundComponent = _stateMachineComponentRD != null ? _stateMachineComponentRD.Owner.GetComponentRD<T>() : default;
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
        public void InitializeTask(IStateMachine stateMachine)
        {
            if (isInitialized)
                return;

            isInitialized = true;

            //set state machine and init
            _stateMachine = stateMachine;
            _stateMachineComponentRD = _stateMachine as IComponentRD;
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