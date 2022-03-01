using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Exit Feedback")]
    public class ExitFeedbacks : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] ExitInteractable interactable;

        [Header("Animator - default get in children")]
        [SerializeField] Animator anim = default;
        [SerializeField] string boolParameter = "IsOpen";

        void OnEnable()
        {
            //get references
            if (interactable == null) interactable = GetComponentInParent<ExitInteractable>();
            if (anim == null) anim = GetComponentInChildren<Animator>();

            //add events
            if (interactable)
            {
                interactable.onOpen += OnSetState;
                interactable.onClose += OnSetState;

                //set default value
                OnSetState();
            }
        }

        void OnDisable()
        {
            //remove events
            if (interactable)
            {
                interactable.onOpen -= OnSetState;
                interactable.onClose -= OnSetState;
            }
        }

        void OnSetState()
        {
            //move to open or close animation
            if (anim)
            {
                anim.SetBool(boolParameter, interactable.IsOpen);
            }
        }
    }
}