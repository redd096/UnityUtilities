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

        protected Transform transformTask => stateMachine ? stateMachine.transform : transform;     //as transform use statemachine if possible, else use this task

        protected T GetStateMachineComponent<T>(bool showWarningIfNotFound = true)
        {
            //get in parent
            T component = GetComponentInParent<T>();

            //show warning if not found
            if (showWarningIfNotFound && component == null)
                Debug.LogWarning($"Miss {typeof(T)} on {stateMachine}");

            return component;
        }

        #endregion

        bool isInitialized;

        /// <summary>
        /// Called by StateMachine, to set owner and to be sure to initialize only one time
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
        /// Called only one time, first time enter in this task
        /// </summary>
        protected virtual void OnInitTask() { }
        /// <summary>
        /// Called every time enter in this task
        /// </summary>
        public virtual void OnEnterTask() { isTaskActive = true; }
        /// <summary>
        /// Called every time exit from this task
        /// </summary>
        public virtual void OnExitTask() { isTaskActive = false; }
    }
}