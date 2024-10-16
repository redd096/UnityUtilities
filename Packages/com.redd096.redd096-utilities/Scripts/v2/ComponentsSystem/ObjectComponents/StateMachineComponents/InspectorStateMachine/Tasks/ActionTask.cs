
namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Inherit from this class to create an Action, where you can set what statemachine must do when in this state
    /// </summary>
    public abstract class ActionTask : BaseTask
    {
        /// <summary>
        /// Can call CompleteTask() to automatically call this event
        /// </summary>
        public System.Action onCompleteTask { get; set; }

        #region statemachine functions

        /// <summary>
        /// Called by StateMachine, to be sure the task is active before call OnUpdateTask()
        /// </summary>
        public void UpdateTask()
        {
            if (IsTaskActive)
                OnUpdateTask();
        }

        /// <summary>
        /// Called by StateMachine, to be sure the task is active before call OnFixedUpdateTask()
        /// </summary>
        public void FixedUpdateTask()
        {
            if (IsTaskActive)
                OnFixedUpdateTask();
        }

        /// <summary>
        /// Called by StateMachine, to be sure the task is active before call OnLateUpdateTask()
        /// </summary>
        public void LateUpdateTask()
        {
            if (IsTaskActive)
                OnLateUpdateTask();
        }

        #endregion

        /// <summary>
        /// Called every frame when inside this task
        /// </summary>
        protected virtual void OnUpdateTask() { }

        /// <summary>
        /// Called every FixedUpdate when inside this task
        /// </summary>
        protected virtual void OnFixedUpdateTask() { }

        /// <summary>
        /// Called every LateUpdate when inside this task
        /// </summary>
        protected virtual void OnLateUpdateTask() { }

        /// <summary>
        /// Call onCompleteTask delegate
        /// </summary>
        protected void CompleteTask()
        {
            onCompleteTask?.Invoke();
        }
    }
}