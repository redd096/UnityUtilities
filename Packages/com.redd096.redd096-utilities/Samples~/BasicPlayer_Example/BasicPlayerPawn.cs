using redd096.Game3D;
using UnityEngine;

namespace redd096.Examples.BasicPlayer
{
    /// <summary>
    /// This is just for example. Normally there is a StateMachine and the statemachine will access to the pawn's controller to get inputs
    /// </summary>
    [AddComponentMenu("redd096/Examples/BasicPlayer/Basic PlayerPawn")]
    public class BasicPlayerPawn : PlayerPawn
    {
        [SerializeField] bool lockMouseOnAwake = true;
        [SerializeField] float delayBeforeEnableComponents = 0.1f;
        [SerializeField] MovementComponent movementComponent;
        [SerializeField] RotationComponent rotationComponent;
        [SerializeField] InteractComponent interactComponent;

        BasicPlayerController controller;
        float delayTime;

        private void Awake()
        {
            delayTime = Time.time + delayBeforeEnableComponents;

            //lock mouse if necessary
            if (lockMouseOnAwake)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public override void OnPossess(PlayerController newController)
        {
            base.OnPossess(newController);
            controller = (BasicPlayerController)newController;
        }

        private void Update()
        {
            //a little delay to not start scene with camera rotated
            if (Time.time < delayTime)
                return;

            //update components
            if (movementComponent) movementComponent.MoveByInput3D(controller.Move);
            if (rotationComponent) rotationComponent.RotateByInput3D(controller.Rotate);
            if (interactComponent) interactComponent.InteractByInput(controller.InteractPressedThisFrame);
        }
    }
}