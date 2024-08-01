using UnityEngine;

namespace redd096.ComponentsSystem
{
    /// <summary>
    /// This Action is used by InspectorStateMachineComponent to move a PlayerPawn by input
    /// </summary>
    [AddComponentMenu("redd096/.ComponentsSystem/FSM Tasks/MovementByInput Action")]
    public class MovementByInputAction : ActionTask
    {
        PlayerPawn player;
        MovementComponent2D movementComponent;
        InputManager inputManager;

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
            inputManager = player.CurrentController ? player.CurrentController.GetComponent<InputManager>() : null;
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