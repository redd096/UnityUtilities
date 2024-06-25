using redd096.Game3D;
using redd096.InspectorStateMachine;
using UnityEngine;
using UnityEngine.UI;

namespace redd096.Examples.InspectorStateMachine_And_Game3D
{
    /// <summary>
    /// Set RawImage and exit from webcam by input
    /// </summary>
    [AddComponentMenu("redd096/Examples/InspectorStateMachine_And_Game3D/Actions/Webcam Action")]
    public class WebcamAction : ActionTask
    {
        [SerializeField] PlayerPawn playerPawn;
        [SerializeField] InteractComponent interactComponent;
        [SerializeField] GameObject canvas;
        [SerializeField] RawImage rawImage;

        InputManager inputManager;

        protected override void OnInitTask()
        {
            base.OnInitTask();

            //get references if null
            if (playerPawn == null && TryGetStateMachineComponent(out playerPawn) == false)
                Debug.LogWarning("Missing playerPawn on " + StateMachine.name, gameObject);
            if (interactComponent == null && TryGetComponent(out interactComponent) == false)
                Debug.LogWarning("Missing interactComponent on " + StateMachine.name, gameObject);
            if (canvas == null)
                Debug.LogWarning("Missing canvas on " + StateMachine.name, gameObject);
            if (rawImage == null)
                Debug.LogWarning("Missing rawImage on " + StateMachine.name, gameObject);
        }

        protected override void OnEnterTask()
        {
            //get input manager from player controller
            inputManager = playerPawn && playerPawn.CurrentController ? playerPawn.CurrentController.GetComponent<InputManager>() : null;

            //set RawImage and active it
            WebcamInteractable webcamInteractable = StateMachine.GetBlackboardElement<WebcamInteractable>("Webcam");
            if (webcamInteractable != null)
            {
                rawImage.texture = webcamInteractable.renderTexture;
                canvas.SetActive(true);
            }
        }

        public override void OnUpdateTask()
        {
            //check if interact again
            if (interactComponent) interactComponent.InteractByInput(inputManager.InteractPressedThisFrame);

            //check also if press Esc to quit, instead of interact again
            if (interactComponent) interactComponent.InteractByInput(inputManager.QuitWebcamPressedThisFrame);
        }

        protected override void OnExitTask()
        {
            //deactive RawImage
            rawImage.texture = null;
            canvas.SetActive(false);
        }
    }
}