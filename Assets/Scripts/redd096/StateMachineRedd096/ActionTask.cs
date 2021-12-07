using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/StateMachineRedd096/Action Task")]
    public class ActionTask : BaseTask
    {
        /// <summary>
        /// Called every frame when inside this task
        /// </summary>
        public virtual void OnUpdateTask() { }
    }
}