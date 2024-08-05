using redd096.v1.InspectorStateMachine;
using UnityEngine;

namespace redd096.Examples.InspectorStateMachine_And_Game3D
{
    /// <summary>
    /// Example of condition. It's used to change from Normal to VacuumState and viceversa
    /// </summary>
    [AddComponentMenu("redd096/Examples/InspectorStateMachine_And_Game3D/Conditions/Check Vacuum Condition")]
    public class CheckVacuumCondition : ConditionTask
    {
        [Tooltip("Return true if vacuum is set in blackboard, or return true when it's NOT set?")][SerializeField] bool returnTrueWhenSet = true;
        [SerializeField] VarOrBlackboard<VacuumInteractable> vacuum = new VarOrBlackboard<VacuumInteractable>("Vacuum");

        public override bool OnCheckTask()
        {
            //return if vacuum is setted in blackboard
            bool isInspector = vacuum.GetValue(StateMachine) != null;
            return returnTrueWhenSet ? isInspector : !isInspector;
        }
    }
}