using redd096.Game3D;
using redd096.InspectorStateMachine;
using UnityEngine;

namespace redd096.Examples.InspectorStateMachine_And_Game3D
{
    /// <summary>
    /// Press second interact (right click) to vacuum objects with VacuumInteractable
    /// </summary>
    [AddComponentMenu("redd096/Examples/InspectorStateMachine_And_Game3D/Actions/Vacuum Action")]
    public class VacuumAction : ActionTask
    {
        [SerializeField] PlayerPawn playerPawn;
        [SerializeField] VarOrBlackboard<VacuumInteractable> vacuum = new VarOrBlackboard<VacuumInteractable>("Vacuum");

        InputManager inputManager;
        VacuumInteractable vacuumInteractable;

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //get references if null
            if (playerPawn == null && TryGetStateMachineComponent(out playerPawn) == false)
                Debug.LogWarning("Missing playerPawn on " + StateMachine.name, gameObject);
        }

        protected override void OnEnterTask()
        {
            base.OnEnterTask();

            //get input manager from player controller
            inputManager = playerPawn && playerPawn.CurrentController ? playerPawn.CurrentController.GetComponent<InputManager>() : null;

            //get reference from blackboard or interactable in inspector
            vacuumInteractable = vacuum.GetValue(StateMachine);
        }

        public override void OnFixedUpdateTask()
        {
            base.OnFixedUpdateTask();

            if (vacuumInteractable == null)
                return;

            //check if press to vacuum or not
            if (inputManager.SecondInteractIsPressed)
            {
                vacuumInteractable.Vacuum();
            }
            else
            {
                vacuumInteractable.StopVacuum();
            }
        }
    }
}