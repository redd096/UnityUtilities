using UnityEngine;

namespace redd096
{
    public abstract class BaseTask : MonoBehaviour
    {
        [Header("Task")]
        public string TaskName = "";

        #region protected

        protected StateMachineRedd096 stateMachine;
        protected bool isTaskActive;

        //as transform use statemachine if possible, else use this task
        protected Transform transformTask => stateMachine ? stateMachine.transform : transform;

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
                Debug.LogWarning($"Miss {typeof(T).Name} on {stateMachine}");

            return component;
        }

        /// <summary>
        /// Get blackboard or normal value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        protected T GetValue<T>(VarOrBlackboard<T> v) => v != null ? v.GetValue(stateMachine) : default;

        #endregion

        bool isInitialized;

        #region statemachine functions

        /// <summary>
        /// Called by StateMachine, to set owner and to be sure to call OnInitTask() only one time
        /// </summary>
        /// <param name="stateMachine"></param>
        public void InitializeTask(StateMachineRedd096 stateMachine)
        {
            if (isInitialized)
                return;

            isInitialized = true;

            //set state machine and init
            this.stateMachine = stateMachine;
            OnInitTask();
        }

        /// <summary>
        /// Called by StateMachine, to set task active and call OnEnterTask()
        /// </summary>
        public void EnterTask()
        {
            isTaskActive = true;
            OnEnterTask();
        }

        /// <summary>
        /// Called by StateMachine, to set task not active and call OnExitTask()
        /// </summary>
        public void ExitTask()
        {
            isTaskActive = false;
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
        public virtual void OnEnterTask() { }
        /// <summary>
        /// Called every time exit from this task
        /// </summary>
        public virtual void OnExitTask() { }
    }
}