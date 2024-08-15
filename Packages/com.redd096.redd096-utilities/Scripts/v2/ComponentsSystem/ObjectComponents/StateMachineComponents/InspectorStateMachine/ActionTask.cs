
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

        /// <summary>
        /// Called every frame when inside this task
        /// </summary>
        public virtual void OnUpdateTask() { }

        /// <summary>
        /// Called every FixedUpdate when inside this task
        /// </summary>
        public virtual void OnFixedUpdateTask() { }

        /// <summary>
        /// Called every LateUpdate when inside this task
        /// </summary>
        public virtual void OnLateUpdateTask() { }

        /// <summary>
        /// Call onCompleteTask delegate
        /// </summary>
        protected void CompleteTask()
        {
            onCompleteTask?.Invoke();
        }
    }
}