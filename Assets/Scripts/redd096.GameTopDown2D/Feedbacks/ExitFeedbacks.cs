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
        [SerializeField] string closeAnimation = "Close";
        [SerializeField] string openAnimation = "Open";

        bool isOpen;

        void OnEnable()
        {
            //get references
            if (interactable == null) interactable = GetComponentInParent<ExitInteractable>();
            if (anim == null) anim = GetComponentInChildren<Animator>();

            //add events
            if (interactable)
            {
                interactable.onOpen += OnOpen;
                interactable.onClose += OnClose;

                //open if interactable is open but animator is still closed
                if (interactable.IsOpen && isOpen == false)
                    OnOpen();
                //or close if is close but animator is still opened
                else if (interactable.IsOpen == false && isOpen)
                    OnClose();
            }
        }

        void OnDisable()
        {
            //remove events
            if (interactable)
            {
                interactable.onOpen -= OnOpen;
                interactable.onClose -= OnClose;
            }
        }

        void OnOpen()
        {
            isOpen = true;

            //move to open animation
            if (anim)
            {
                anim.Play(openAnimation);
            }
        }

        void OnClose()
        {
            isOpen = false;

            //move to close animation
            if (anim)
            {
                anim.Play(closeAnimation);
            }
        }
    }
}