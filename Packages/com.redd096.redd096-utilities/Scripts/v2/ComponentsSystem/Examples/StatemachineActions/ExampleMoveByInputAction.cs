using UnityEngine;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// This Action is used by InspectorStateMachineComponent to move a PlayerPawn by input
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Examples/Example MoveByInput Action")]
    public class ExampleMoveByInputAction : ActionTask
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
                Debug.LogError($"Missing MovementComponent2D on {name}", gameObject);
        }

        protected override void OnEnterTask()
        {
            base.OnEnterTask();

            //get InputManager
            if (player.CurrentController == null || player.CurrentController.TryGetComponent(out inputManager) == false)
                Debug.LogError($"Missing inputManager on {name}", gameObject);
        }

        public override void OnUpdateTask()
        {
            base.OnUpdateTask();

            //set move direction
            if (movementComponent != null && inputManager)
            {
                //for 3d just call MoveByInput3D
                movementComponent.MoveInDirection(inputManager.Move);
            }
        }

        public override void OnFixedUpdateTask()
        {
            base.OnFixedUpdateTask();

            //move
            if (movementComponent != null)
                movementComponent.UpdatePosition();
        }
    }
}