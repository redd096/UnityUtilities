using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Idle Run Animation Feedback")]
    public class IdleRunAnimationFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in child and parent")]
        [SerializeField] Animator anim = default;
        [SerializeField] MovementComponent movementComponent = default;

        [Header("Run when speed > this value")]
        [SerializeField] float valueToRun = 0.1f;

        [Header("Animator parameters")]
        [SerializeField] string boolName = "IsRunning";

        bool isRunning;

        void OnEnable()
        {
            if (anim == null) anim = GetComponentInChildren<Animator>();
            if (movementComponent == null) movementComponent = GetComponentInParent<MovementComponent>();
        }

        void Update()
        {
            if (anim && movementComponent)
            {
                //start run
                if (movementComponent.CurrentSpeed > valueToRun && isRunning == false)
                {
                    isRunning = true;

                    //set animator
                    anim.SetBool(boolName, true);
                }
                //back to idle
                else if (movementComponent.CurrentSpeed <= valueToRun && isRunning)
                {
                    isRunning = false;

                    //set animator
                    anim.SetBool(boolName, false);
                }
            }
        }
    }
}