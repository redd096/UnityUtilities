
namespace redd096.v2.ComponentsSystem
{
    /// <summary>
    /// Inherit from this class to create a Condition, where you can set when statemachine must change state
    /// </summary>
    public abstract class ConditionTask : BaseTask
    {
        /// <summary>
        /// Called by StateMachine, to be sure the task is active before call OnCheckTask()
        /// </summary>
        public bool CheckTask()
        {
            if (IsTaskActive)
                return OnCheckTask();

            return false;
        }

        /// <summary>
        /// Called every frame when inside this task. Return true to tell statemachine to change state (check Transition inside State class)
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnCheckTask() { return true; }
    }
}