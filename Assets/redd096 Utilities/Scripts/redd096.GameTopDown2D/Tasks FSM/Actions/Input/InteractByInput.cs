using UnityEngine;
using UnityEngine.InputSystem;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Tasks FSM/Actions/Input/Interact By Input")]
    public class InteractByInput : ActionTask
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] InteractComponent interactComponent = default;
        [SerializeField] PlayerInput playerInput = default;

        [Header("Interact")]
        [SerializeField] string inputName = "Interact";

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //set references
            if (interactComponent == null) interactComponent = GetStateMachineComponent<InteractComponent>();
            if (playerInput == null) playerInput = GetStateMachineComponent<PlayerInput>();

            //show warnings if not found
            if (playerInput && playerInput.actions == null)
                Debug.LogWarning("Miss Actions on PlayerInput on " + stateMachine);
        }

        public override void OnUpdateTask()
        {
            base.OnUpdateTask();

            if (interactComponent == null || playerInput == null || playerInput.actions == null)
                return;

            //when press input, interact
            if (playerInput.actions.FindAction(inputName).triggered)
                interactComponent.Interact();
        }
    }
}