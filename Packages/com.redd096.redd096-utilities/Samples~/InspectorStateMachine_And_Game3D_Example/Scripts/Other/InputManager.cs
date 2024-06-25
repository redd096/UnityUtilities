using redd096.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace redd096.Examples.InspectorStateMachine_And_Game3D
{
    /// <summary>
    /// This is attached to PlayerController, to manage inputs
    /// </summary>
    [AddComponentMenu("redd096/Examples/InspectorStateMachine_And_Game3D/Other/InputManager Example")]
    public class InputManager : MonoBehaviour
    {
        [ReadOnly] public Vector2 Move;
        [ReadOnly] public Vector2 Rotate;
        [ReadOnly] public bool InteractPressedThisFrame;
        [ReadOnly] public bool InteractReleasedThisFrame;
        [ReadOnly] public bool SecondInteractIsPressed;
        [ReadOnly] public bool QuitWebcamPressedThisFrame;

        PlayerInput playerInput;

        private void Awake()
        {
            //get references
            playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            //calculate inputs
            Move = playerInput.actions.FindAction("Move").ReadValue<Vector2>();
            Rotate = playerInput.actions.FindAction("Rotate").ReadValue<Vector2>();
            InteractPressedThisFrame = playerInput.actions.FindAction("Interact").WasPressedThisFrame();
            InteractReleasedThisFrame = playerInput.actions.FindAction("Interact").WasReleasedThisFrame();
            SecondInteractIsPressed = playerInput.actions.FindAction("SecondInteract").IsPressed();
            QuitWebcamPressedThisFrame = playerInput.actions.FindAction("QuitWebcam").WasPerformedThisFrame();
        }
    }
}
