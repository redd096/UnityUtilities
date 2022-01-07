using UnityEngine;
using UnityEngine.InputSystem;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Aim Feedback")]
    public class AimFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] AimComponent aimComponent = default;
        [SerializeField] PlayerInput playerInput = default;

        [Header("Aim Sprite (already in scene/prefab)")]
        [SerializeField] GameObject aimSprite = default;
        [SerializeField] float minDistance = 1.5f;
        [SerializeField] float maxDistance = 2.5f;

        [Header("Mouse free or clamped by min and max?")]
        [SerializeField] bool mouseFree = true;
        [SerializeField] string mouseSchemeName = "KeyboardAndMouse";

        void OnEnable()
        {
            //get references
            if (aimComponent == null) aimComponent = GetComponentInParent<AimComponent>();
            if (playerInput == null) playerInput = GetComponentInParent<PlayerInput>();
        }

        void Update()
        {
            //update sprite position
            UpdateAimSpritePosition();
        }

        void UpdateAimSpritePosition()
        {
            if (aimSprite == null || aimComponent == null)
                return;

            //if using mouse
            if (playerInput && playerInput.currentControlScheme == mouseSchemeName)
            {
                //if mouse free
                if (mouseFree)
                {
                    aimSprite.transform.position = aimComponent.AimPositionNotNormalized;                                           //set current position
                }
                //else check distance
                else
                {
                    float distance = Vector2.Distance(transform.position, aimComponent.AimPositionNotNormalized);

                    if (distance > maxDistance)
                        aimSprite.transform.position = (Vector2)transform.position + aimComponent.AimDirectionInput * maxDistance;  //max distance
                    else if (distance < minDistance)
                        aimSprite.transform.position = (Vector2)transform.position + aimComponent.AimDirectionInput * minDistance;  //min distance
                    else
                        aimSprite.transform.position = aimComponent.AimPositionNotNormalized;                                       //current distance
                }
            }
            //if NOT using mouse
            else
            {
                //from 0 to 1, from min to max (gamepad input is from 0 to 1, so removing transform.position we will get that value)
                float value = Mathf.Lerp(minDistance, maxDistance, (aimComponent.AimPositionNotNormalized - (Vector2)transform.position).magnitude);

                //set position
                aimSprite.transform.position = (Vector2)transform.position + aimComponent.AimDirectionInput * value;
            }
        }
    }
}