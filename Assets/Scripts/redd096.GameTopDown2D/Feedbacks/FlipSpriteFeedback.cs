using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Feedbacks/Flip Sprite Feedback")]
    public class FlipSpriteFeedback : MonoBehaviour
    {
        [Header("Necessary Components - default get in parent")]
        [SerializeField] AimComponent component;

        [Header("Default get component in children")]
        [Tooltip("By default these sprites are looking to the right?")] [SerializeField] bool defaultLookRight = true;
        [SerializeField] SpriteRenderer[] spritesToFlip = default;

        void OnEnable()
        {
            //get references
            if(component == null) component = GetComponentInParent<AimComponent>();
            if (spritesToFlip == null || spritesToFlip.Length <= 0) spritesToFlip = GetComponentsInChildren<SpriteRenderer>();

            //add events
            if (component)
                component.onChangeAimDirection += OnChangeAimDirection;
        }

        void OnDisable()
        {
            //remove events
            if (component)
                component.onChangeAimDirection -= OnChangeAimDirection;
        }

        void OnChangeAimDirection(bool isLookingRight)
        {
            //flip right or left
            foreach (SpriteRenderer sprite in spritesToFlip)
                if(sprite)
                    sprite.flipX = (defaultLookRight && isLookingRight == false) || (defaultLookRight == false && isLookingRight);
        }
    }
}