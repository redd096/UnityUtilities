using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Rotate Left Right Feedback")]
    public class RotateLeftRightFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] AimComponent component;

        [Header("Default this transform")]
        [SerializeField] Transform objectToRotate = default;
        [Tooltip("By default these sprites are looking to the right?")] [SerializeField] bool defaultLookRight = true;

        bool lookRight;

        void Awake()
        {
            //set default rotation
            lookRight = defaultLookRight;
        }

        void OnEnable()
        {
            //get references
            if (component == null) component = GetComponentInParent<AimComponent>();
            if (objectToRotate == null) objectToRotate = transform;

            //add events
            if (component)
            {
                component.onChangeAimDirection += OnChangeAimDirection;
                OnChangeAimDirection(component.IsLookingRight);     //set default rotation
            }
        }

        void OnDisable()
        {
            //remove events
            if (component)
                component.onChangeAimDirection -= OnChangeAimDirection;
        }

        void OnChangeAimDirection(bool isLookingRight)
        {
            //if change direction, rotate
            if ((lookRight && isLookingRight == false) || (lookRight == false && isLookingRight))
            {
                if (objectToRotate) objectToRotate.rotation *= Quaternion.AngleAxis(180, Vector3.up);
                lookRight = isLookingRight;
            }
        }
    }
}