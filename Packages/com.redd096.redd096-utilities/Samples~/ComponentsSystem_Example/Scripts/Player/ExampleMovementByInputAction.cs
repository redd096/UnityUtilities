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
        PlayerPawn player;
        MovementComponent2D movementComponent;
        ExampleInputManager inputManager;

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //get references
            if (player == null && TryGetStateMachineComponent(out player) == false)
                Debug.LogError($"Missing PlayerPawn on {name}", gameObject);
            if (movementComponent == null && StateMachine.Owner.TryGetCharacterComponent(out movementComponent) == false)
                Debug.LogError($"Missing MovementComponent2D on {name}", gameObject);
        }

        protected override void OnEnterTask()
        {
            base.OnEnterTask();

            //get InputManager
            inputManager = player.CurrentController ? player.CurrentController.GetComponent<ExampleInputManager>() : null;
            if (inputManager == null) Debug.LogError($"Missing inputManager on {name}", gameObject);
        }

        public override void OnUpdateTask()
        {
            base.OnUpdateTask();

            //move
            if (movementComponent != null && inputManager)
            {
                movementComponent.MoveInDirection(inputManager.Movement);
            }
        }
    }
}