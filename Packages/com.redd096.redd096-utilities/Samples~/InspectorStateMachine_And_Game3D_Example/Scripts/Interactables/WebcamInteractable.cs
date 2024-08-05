using redd096.v1.Game3D;
using redd096.v1.InspectorStateMachine;
using UnityEngine;

namespace redd096.Examples.InspectorStateMachine_And_Game3D
{
    /// <summary>
    /// This is an example of how you can set manually the state in Statemachine
    /// </summary>
    [AddComponentMenu("redd096/Examples/InspectorStateMachine_And_Game3D/Interactables/Webcam Interactable")]
    public class WebcamInteractable : MonoBehaviour, IInteractable
    {
        public RenderTexture renderTexture;

        StateMachine interactorStateMachine;

        public bool OnInteract(InteractComponent interactor, RaycastHit hit, params object[] args)
        {
            //set interactor to be in WebcamState
            interactorStateMachine = interactor.GetComponent<StateMachine>();
            if (interactorStateMachine != null)
            {
                interactorStateMachine.SetBlackboardElement("Webcam", this);    //set reference to this webcam
                interactorStateMachine.SetState("WebcamState");
            }

            return true;
        }

        public bool OnDismiss(InteractComponent interactor, params object[] args)
        {
            //reset interactor on Dismiss
            if (interactorStateMachine != null)
            {
                interactorStateMachine.RemoveBlackboardElement("Webcam");       //remove reference
                interactorStateMachine.SetState(0);
            }

            return true;
        }
    }
}