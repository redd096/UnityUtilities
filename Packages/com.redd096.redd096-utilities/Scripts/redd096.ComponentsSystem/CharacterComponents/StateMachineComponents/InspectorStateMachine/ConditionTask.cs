
namespace redd096.ComponentsSystem
{
    /// <summary>
    /// Inherit from this class to create a Condition, where you can set when statemachine must change state
    /// </summary>
    public abstract class ConditionTask : BaseTask
    {
        /// <summary>
        /// Called every frame when inside this task. Return true to tell statemachine to change state (check Transition inside State class)
        /// </summary>
        /// <returns></returns>
        public virtual bool OnCheckTask() { return true; }
    }
}