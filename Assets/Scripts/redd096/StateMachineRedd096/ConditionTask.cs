using UnityEngine;

namespace redd096
{
    [AddComponentMenu("redd096/StateMachineRedd096/Condition Task")]
    public class ConditionTask : BaseTask
    {
        /// <summary>
        /// Called every frame when inside this task
        /// </summary>
        /// <returns></returns>
        public virtual bool OnCheckTask() { return true; }
    }
}