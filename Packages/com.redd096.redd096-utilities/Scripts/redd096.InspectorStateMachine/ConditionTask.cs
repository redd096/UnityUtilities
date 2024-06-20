using UnityEngine;

namespace redd096.InspectorStateMachine
{
    [AddComponentMenu("redd096/.InspectorStateMachine/Condition Task")]
    public class ConditionTask : BaseTask
    {
        /// <summary>
        /// Called every frame when inside this task
        /// </summary>
        /// <returns></returns>
        public virtual bool OnCheckTask() { return true; }
    }
}