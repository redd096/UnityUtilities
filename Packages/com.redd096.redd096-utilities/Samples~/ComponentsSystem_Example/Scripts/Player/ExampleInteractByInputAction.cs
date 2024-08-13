using redd096.v2.ComponentsSystem;
using UnityEngine;

namespace redd096.Examples.ComponentsSystem
{
    /// <summary>
    /// This Action is used to call Interact on input
    /// </summary>
    [AddComponentMenu("redd096/Examples/ComponentsSystem/Player/Example InteractByInput Action")]
    public class ExampleInteractByInputAction : ActionTask
    {
        PlayerPawn player;
        InteractComponent2D interactComponent;
        ExampleInputManager inputManager;

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //get references
            if (player == null && TryGetStateMachineComponent(out player) == false)
                Debug.LogError($"Missing PlayerPawn on {TaskName}", gameObject);
            if (interactComponent == null && TryGetCharacterComponent(out interactComponent) == false)
                Debug.LogError($"Missing interactComponent on {TaskName}", gameObject);
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

            if (inputManager && inputManager.InteractWasPressedThisFrame)
            {
                if (interactComponent != null)
                    interactComponent.Interact();
            }
        }
    }
}