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
            if (player == null && TryGetStateMachineUnityComponent(out player) == false)
                Debug.LogError($"Missing PlayerPawn on {name}", gameObject);
            if (aimComponent == null && TryGetOwnerComponent(out aimComponent) == false)
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
                    //for 3d use CalculateAimPositionWithMouse
                    aimComponent.AimAt(cam.ScreenToWorldPoint(inputManager.MousePosition));
                }
            }
            //or using analog
            else
            {
                //check if moving analog or reset input when released
                if (inputManager.Aim != Vector2.zero || resetWhenReleaseAnalogInput)
                {
                    //for 3d just call AimInDirectionByInput3D
                    aimComponent.AimInDirection(inputManager.Aim);
                    lastAnalogSavedValue = inputManager.Aim;    //save input
                }
                //else show last saved input
                else
                {
                    //for 3d just call AimInDirectionByInput3D
                    aimComponent.AimInDirection(lastAnalogSavedValue);
                }
            }
        }

        //void CalculateAimPositionWithMouse()
        //{
        //    //use raycast to find where is the mouse (if hit nothing, use direction)
        //    if (useRaycastForMouse)
        //    {
        //        Ray ray = cam.ScreenPointToRay(inputManager.MousePosition);
        //        if (Physics.Raycast(ray, out RaycastHit hit))
        //        {
        //            aimComponent.AimAt(hit.point);
        //            return;
        //        }
        //    }

        //    //calculate direction from player to mouse position
        //    aimComponent.AimInDirectionByInput3D(CalculateDirectionWithScreenPoints());
        //}

        //Vector2 CalculateDirectionWithScreenPoints()
        //{
        //    //from owner screen position to mouse position
        //    Vector3 ownerPos = cam.WorldToScreenPoint(transformTask.position);
        //    return (inputManager.MousePosition - (Vector2)ownerPos).normalized;
        //}
    }
}