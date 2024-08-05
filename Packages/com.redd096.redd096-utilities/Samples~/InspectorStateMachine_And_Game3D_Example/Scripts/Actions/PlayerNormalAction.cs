using redd096.v1.Game3D;
using redd096.v1.InspectorStateMachine;
using UnityEngine;

namespace redd096.Examples.InspectorStateMachine_And_Game3D
{
    /// <summary>
    /// In normal state, update player components by input
    /// </summary>
    [AddComponentMenu("redd096/Examples/InspectorStateMachine_And_Game3D/Actions/Player Normal Action")]
    public class PlayerNormalAction : ActionTask
    {
        [SerializeField] PlayerPawn playerPawn;
        [SerializeField] MovementComponent movementComponent;
        [SerializeField] RotationComponent rotationComponent;
        [SerializeField] InteractComponent interactComponent;

        InputManager inputManager;

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //get references if null
            if (playerPawn == null && TryGetStateMachineComponent(out playerPawn) == false)
                Debug.LogWarning("Missing playerPawn on " + StateMachine.name, gameObject);
            if (movementComponent == null && TryGetComponent(out movementComponent) == false)
                Debug.LogWarning("Missing movementComponent on " + StateMachine.name, gameObject);
            if (rotationComponent == null && TryGetComponent(out rotationComponent) == false)
                Debug.LogWarning("Missing rotationComponent on " + StateMachine.name, gameObject);
            if (interactComponent == null && TryGetComponent(out interactComponent) == false)
                Debug.LogWarning("Missing interactComponent on " + StateMachine.name, gameObject);
        }

        protected override void OnEnterTask()
        {
            base.OnEnterTask();

            //get input manager from player controller
            inputManager = playerPawn && playerPawn.CurrentController ? playerPawn.CurrentController.GetComponent<InputManager>() : null;
        }

        public override void OnUpdateTask()
        {
            base.OnUpdateTask();

            //update components
            if (movementComponent) movementComponent.MoveByInput3D(inputManager.Move);
            if (rotationComponent) rotationComponent.RotateByInput3D(inputManager.Rotate);
            if (interactComponent) interactComponent.InteractByInput(inputManager.InteractPressedThisFrame);
            if (interactComponent) interactComponent.ReleaseInteractInput(inputManager.InteractReleasedThisFrame);
        }
    }
}