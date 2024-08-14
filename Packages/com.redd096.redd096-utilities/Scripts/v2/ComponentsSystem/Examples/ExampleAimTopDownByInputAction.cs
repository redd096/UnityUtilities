using UnityEngine;
using UnityEngine.InputSystem;

namespace redd096.v2.ComponentsSystem.Example
{
    /// <summary>
    /// This Action is used by InspectorStateMachineComponent to aim by input in a topdown game
    /// </summary>
    [AddComponentMenu("redd096/v2/ComponentsSystem/Examples/Example AimTopDownByInput Action")]
    public class ExampleAimTopDownByInputAction : ActionTask
    {
        [Header("Default cam is MainCamera")]
        [SerializeField] Camera cam = default;
        [SerializeField] string mouseSchemeName = "MouseAndKeyboard";
        [SerializeField] bool resetWhenReleaseAnalogInput = false;

        PlayerPawn player;
        TopDownAimComponent aimComponent;
        ExampleInputManager inputManager;
        PlayerInput playerInput;

        Vector2 lastAnalogSavedValue = Vector2.right;

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //get references
            if (player == null && TryGetStateMachineComponent(out player) == false)
                Debug.LogError($"Missing PlayerPawn on {name}", gameObject);
            if (aimComponent == null && TryGetCharacterComponent(out aimComponent) == false)
                Debug.LogError($"Missing aimComponent on {name}", gameObject);
            if (cam == null) cam = Camera.main;
            if (cam == null) Debug.LogError($"Missing camera on {name}", gameObject);
        }

        protected override void OnEnterTask()
        {
            base.OnEnterTask();

            //get InputManager and PlayerInput
            if (player.CurrentController == null || player.CurrentController.TryGetComponent(out inputManager) == false)
                Debug.LogError($"Missing inputManager on {name}", gameObject);
            if (player.CurrentController == null || player.CurrentController.TryGetComponent(out playerInput) == false)
                Debug.LogError($"Missing playerInput on {name}", gameObject);
        }

        public override void OnUpdateTask()
        {
            base.OnUpdateTask();

            if (aimComponent == null || inputManager == null)
                return;

            //set direction using mouse position
            if (playerInput != null && playerInput.currentControlScheme == mouseSchemeName)
            {
                if (cam)
                {
                    //for 3d use a raycast from camera to mouse position and use hit point as position for AimAtByInput3D
                    //or just calculate direction from player to mouse and use AimInDirectionByInput3D
                    aimComponent.AimAt(cam.ScreenToWorldPoint(inputManager.MousePosition));
                }
            }
            //or using analog
            else
            {
                //check if moving analog or reset input when released
                if (inputManager.Aim != Vector2.zero || resetWhenReleaseAnalogInput)
                {
                    aimComponent.AimInDirection(inputManager.Aim);
                    lastAnalogSavedValue = inputManager.Aim;    //save input
                }
                //else show last saved input
                else
                {
                    aimComponent.AimInDirection(lastAnalogSavedValue);
                }
            }
        }
    }
}