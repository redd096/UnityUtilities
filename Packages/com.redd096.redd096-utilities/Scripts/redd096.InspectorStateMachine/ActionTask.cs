using UnityEngine;

namespace redd096.InspectorStateMachine
{
    /// <summary>
    /// Inherit from this class to create an Action, where you can set what statemachine must do when in this state
    /// </summary>
    [AddComponentMenu("redd096/.InspectorStateMachine/Action Task")]
    public class ActionTask : BaseTask
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
        /// Call onCompleteTask delegate
        /// </summary>
        protected void CompleteTask()
        {
            onCompleteTask?.Invoke();
        }
    }
}