using UnityEngine;

namespace redd096.InspectorStateMachine
{
    public abstract class BaseTask : MonoBehaviour
    {
        [Header("Task")]
        public string TaskName = "";

        private StateMachine _stateMachine;
        public StateMachine StateMachine { get => _stateMachine; set => _stateMachine = value; }
        private bool _isTaskActive;
        public bool IsTaskActive { get => _isTaskActive; set => _isTaskActive = value; }

        bool isInitialized;

        /// <summary>
        /// Return StateMachine casted to inherited class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetStateMachine<T>() where T : StateMachine
        {
            return _stateMachine as T;
        }

        #region protected

        //as transform use stateMachine if possible, else use this task
        protected Transform transformTask => _stateMachine ? _stateMachine.transform : transform;

        /// <summary>
        /// Get component in parent. If not found, show warning
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="showWarningIfNotFound"></param>
        /// <returns></returns>
        protected T GetStateMachineComponent<T>(bool showWarningIfNotFound = true)
        {
            //get in parent
            T component = GetComponentInParent<T>();

            //show warning if not found
            if (showWarningIfNotFound && component == null)
                Debug.LogWarning($"Miss {typeof(T).Name} on {_stateMachine}");

            return component;
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
        public void InitializeTask(StateMachine stateMachine)
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