using UnityEngine;
using UnityEngine.InputSystem;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Tasks FSM/Actions/Input/Move By Input")]
    public class MoveByInput : ActionTask
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] MovementComponent movementComponent = default;
        [SerializeField] PlayerInput playerInput = default;

        [Header("Movement")]
        [SerializeField] string inputName = "Move";

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //set references
            if (movementComponent == null) movementComponent = GetStateMachineComponent<MovementComponent>();
            if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

            //show warnings if not found
            if (playerInput && playerInput.actions == null)
                Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
        }

        public override void OnUpdateTask()
        {
            base.OnUpdateTask();

            if (movementComponent == null || playerInput == null || playerInput.actions == null)
                return;

            //move in direction
            movementComponent.MoveInDirection(playerInput.actions.FindAction(inputName).ReadValue<Vector2>());
        }
    }
}