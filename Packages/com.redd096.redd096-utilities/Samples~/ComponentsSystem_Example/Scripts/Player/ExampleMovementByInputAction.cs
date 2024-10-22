using UnityEngine;
using redd096.v2.ComponentsSystem;

namespace redd096.Examples.ComponentsSystem
{
    /// <summary>
    /// This Action is used by InspectorStateMachineComponent to move a PlayerPawn by input
    /// </summary>
    [AddComponentMenu("redd096/Examples/ComponentsSystem/Player/Example MovementByInput Action")]
    public class ExampleMovementByInputAction : ActionTask
    {
        SimplePlayerPawn player;
        SimpleMovementComponent movementComponent;
        ExampleInputManager inputManager;

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //get references
            if (player == null && TryGetStateMachineUnityComponent(out player) == false)
                Debug.LogError($"Missing PlayerPawn on {name}", gameObject);
            if (movementComponent == null && TryGetStateMachineComponentRD(out movementComponent) == false)
                Debug.LogError($"Missing MovementComponent on {name}", gameObject);
        }

        protected override void OnEnterTask()
        {
            base.OnEnterTask();

            //get InputManager
            inputManager = player.CurrentController ? player.CurrentController.GetComponent<ExampleInputManager>() : null;
            if (inputManager == null) Debug.LogError($"Missing inputManager on {name}", gameObject);
        }

        protected override void OnUpdateTask()
        {
            base.OnUpdateTask();

            //set move direction
            if (movementComponent != null && inputManager)
            {
                movementComponent.MoveInDirection(inputManager.Movement);
            }
        }

        protected override void OnFixedUpdateTask()
        {
            base.OnFixedUpdateTask();

            //move
            if (movementComponent != null)
                movementComponent.UpdatePosition();
        }
    }
}